-- =============================================
-- QUẢN LÝ NHÂN VIÊN QUÁN ĂN
-- Migration 002: Shift Scheduling, Attendance,
--   Bonus/Penalty, and Payroll Stored Procedures
--
-- Target: SQL Server 2019+
-- Author: Auto-generated
-- Date:   2026-02-14
-- =============================================

USE QuanLyNhanVien;
GO

-- Required for persisted computed columns and indexed views
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- 1. CaLamViec — Shift definitions (master data)
--    Restaurant shifts: Morning prep, Lunch,
--    Dinner, Late-night
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'CaLamViec' AND xtype = 'U')
BEGIN
    CREATE TABLE CaLamViec (
        MaCa        INT IDENTITY(1,1) PRIMARY KEY,
        TenCa       NVARCHAR(50)  NOT NULL UNIQUE,
        GioBatDau   TIME          NOT NULL,
        GioKetThuc  TIME          NOT NULL,
        HeSoLuong   DECIMAL(3,2)  NOT NULL DEFAULT 1.00,
            -- 1.00 = normal, 1.50 = overtime/night
        MoTa        NVARCHAR(200) NULL,
        TrangThai   NVARCHAR(20)  NOT NULL DEFAULT N'Hoạt động',
            -- 'Hoạt động' | 'Ngừng sử dụng'
        NgayTao     DATETIME2     NOT NULL DEFAULT SYSDATETIME()
    );
END
GO

-- =============================================
-- 2. LichLamViec — Shift schedule / assignment
--    Plans who works which shift on which date.
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'LichLamViec' AND xtype = 'U')
BEGIN
    CREATE TABLE LichLamViec (
        MaLich      INT IDENTITY(1,1) PRIMARY KEY,
        MaNV        INT           NOT NULL,
        MaCa        INT           NOT NULL,
        NgayLamViec DATE          NOT NULL,
        GhiChu      NVARCHAR(200) NULL,
        NgayTao     DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_LichLamViec_NhanVien FOREIGN KEY (MaNV)
            REFERENCES NhanVien(MaNV),
        CONSTRAINT FK_LichLamViec_CaLamViec FOREIGN KEY (MaCa)
            REFERENCES CaLamViec(MaCa),
        -- One employee cannot be assigned the same shift
        -- on the same day twice
        CONSTRAINT UQ_LichLamViec_NV_Ca_Ngay
            UNIQUE (MaNV, MaCa, NgayLamViec)
    );
END
GO

-- =============================================
-- 3. ChamCong — Attendance logs
--    Records actual check-in / check-out times.
--    TrangThai indicates presence quality.
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ChamCong' AND xtype = 'U')
BEGIN
    CREATE TABLE ChamCong (
        MaChamCong  INT IDENTITY(1,1) PRIMARY KEY,
        MaNV        INT           NOT NULL,
        NgayLamViec DATE          NOT NULL,
        MaCa        INT           NULL,
            -- Nullable: attendance may not always map to a shift
        GioVao      TIME          NULL,
        GioRa       TIME          NULL,
        SoGioLam    AS (
            -- Computed column: actual hours worked
            CASE
                WHEN GioVao IS NOT NULL AND GioRa IS NOT NULL
                THEN CAST(DATEDIFF(MINUTE, GioVao, GioRa) / 60.0 AS DECIMAL(5,2))
                ELSE NULL
            END
        ) PERSISTED,
        TrangThai   NVARCHAR(20)  NOT NULL DEFAULT N'Có mặt',
            -- 'Có mặt' | 'Vắng' | 'Trễ' | 'Về sớm' | 'Nghỉ phép'
        GhiChu      NVARCHAR(200) NULL,
        NgayTao     DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_ChamCong_NhanVien FOREIGN KEY (MaNV)
            REFERENCES NhanVien(MaNV),
        CONSTRAINT FK_ChamCong_CaLamViec FOREIGN KEY (MaCa)
            REFERENCES CaLamViec(MaCa),
        -- One attendance record per employee per shift per day
        CONSTRAINT UQ_ChamCong_NV_Ca_Ngay
            UNIQUE (MaNV, MaCa, NgayLamViec)
    );
END
GO

-- =============================================
-- 4. ThuongPhat — Bonus / Penalty management
--    Tracks all additions and deductions from
--    employee pay beyond standard salary.
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ThuongPhat' AND xtype = 'U')
BEGIN
    CREATE TABLE ThuongPhat (
        MaThuongPhat INT IDENTITY(1,1) PRIMARY KEY,
        MaNV         INT           NOT NULL,
        Thang        INT           NOT NULL CHECK (Thang BETWEEN 1 AND 12),
        Nam          INT           NOT NULL CHECK (Nam >= 2020),
        Loai         NVARCHAR(10)  NOT NULL,
            -- N'Thưởng' | N'Phạt'
        SoTien       DECIMAL(18,0) NOT NULL CHECK (SoTien > 0),
        LyDo         NVARCHAR(500) NOT NULL,
        NgayTao      DATETIME2     NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_ThuongPhat_NhanVien FOREIGN KEY (MaNV)
            REFERENCES NhanVien(MaNV),
        CONSTRAINT CK_ThuongPhat_Loai
            CHECK (Loai IN (N'Thưởng', N'Phạt'))
    );
END
GO

-- =============================================
-- 5. Expand BangLuong — add columns for new data
--    so the payroll snapshot is self-contained.
-- =============================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('BangLuong')
      AND name = 'SoNgayChamCong'
)
BEGIN
    ALTER TABLE BangLuong ADD
        SoNgayChamCong  DECIMAL(5,1) NULL DEFAULT 0,
            -- Days from attendance (ChamCong)
        TongGioLam      DECIMAL(7,2) NULL DEFAULT 0,
            -- Total hours from attendance
        TienThuong      DECIMAL(18,0) NULL DEFAULT 0,
            -- Sum of bonuses this month
        TienPhat        DECIMAL(18,0) NULL DEFAULT 0,
            -- Sum of penalties this month
        HeSoTrungBinh   DECIMAL(3,2) NULL DEFAULT 1.00,
            -- Weighted-average shift multiplier
        NgayCapNhat     DATETIME2 NULL;
            -- When this record was last regenerated
END
GO

-- =============================================
-- 6. INDEXES — Optimized for the query patterns
--    used by stored procedures and the UI.
-- =============================================

-- ChamCong: most queries filter by (MaNV, NgayLamViec)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ChamCong_NV_Ngay'
      AND object_id = OBJECT_ID('ChamCong')
)
    CREATE NONCLUSTERED INDEX IX_ChamCong_NV_Ngay
        ON ChamCong (MaNV, NgayLamViec)
        INCLUDE (TrangThai, SoGioLam, MaCa);
GO

-- ChamCong: monthly payroll aggregation
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ChamCong_Thang'
      AND object_id = OBJECT_ID('ChamCong')
)
    CREATE NONCLUSTERED INDEX IX_ChamCong_Thang
        ON ChamCong (NgayLamViec, MaNV)
        INCLUDE (TrangThai, SoGioLam, MaCa);
GO

-- LichLamViec: schedule lookup by employee and date
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_LichLamViec_NV_Ngay'
      AND object_id = OBJECT_ID('LichLamViec')
)
    CREATE NONCLUSTERED INDEX IX_LichLamViec_NV_Ngay
        ON LichLamViec (MaNV, NgayLamViec)
        INCLUDE (MaCa);
GO

-- ThuongPhat: monthly aggregation by employee
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ThuongPhat_NV_ThangNam'
      AND object_id = OBJECT_ID('ThuongPhat')
)
    CREATE NONCLUSTERED INDEX IX_ThuongPhat_NV_ThangNam
        ON ThuongPhat (MaNV, Thang, Nam)
        INCLUDE (Loai, SoTien);
GO

-- =============================================
-- 7. SAMPLE DATA — Shifts and attendance for
--    testing the stored procedures.
-- =============================================

-- 7a. Shift definitions
IF NOT EXISTS (SELECT 1 FROM CaLamViec)
BEGIN
    INSERT INTO CaLamViec (TenCa, GioBatDau, GioKetThuc, HeSoLuong, MoTa) VALUES
    (N'Ca sáng',  '06:00', '14:00', 1.00, N'Ca chuẩn bị và phục vụ bữa trưa'),
    (N'Ca chiều', '14:00', '22:00', 1.00, N'Ca phục vụ bữa tối'),
    (N'Ca tối',   '18:00', '23:00', 1.20, N'Ca cao điểm buổi tối — hệ số 1.2'),
    (N'Ca khuya', '22:00', '06:00', 1.50, N'Ca đêm — hệ số 1.5');
END
GO

-- 7b. Shift schedule for February 2026 (first 10 days)
-- Assign employees to shifts for testing
IF NOT EXISTS (SELECT 1 FROM LichLamViec WHERE NgayLamViec >= '2026-02-01' AND NgayLamViec < '2026-03-01')
BEGIN
    DECLARE @d DATE = '2026-02-01';
    WHILE @d <= '2026-02-10'
    BEGIN
        -- Chef (MaNV=1): morning shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (1, 1, @d);
        -- Sous-chef (MaNV=2): morning shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (2, 1, @d);
        -- Wait staff (MaNV=3): afternoon shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (3, 2, @d);
        -- Wait staff (MaNV=4): afternoon shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (4, 2, @d);
        -- Cashier (MaNV=5): morning shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (5, 1, @d);
        -- Security (MaNV=6): evening high-load shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (6, 3, @d);
        -- Manager (MaNV=7): morning shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (7, 1, @d);
        -- Seafood chef (MaNV=8): evening high-load shift
        INSERT INTO LichLamViec (MaNV, MaCa, NgayLamViec) VALUES (8, 3, @d);

        SET @d = DATEADD(DAY, 1, @d);
    END
END
GO

-- 7c. Attendance records for February 2026
IF NOT EXISTS (SELECT 1 FROM ChamCong WHERE MONTH(NgayLamViec) = 2 AND YEAR(NgayLamViec) = 2026)
BEGIN
    DECLARE @ad DATE = '2026-02-01';
    WHILE @ad <= '2026-02-10'
    BEGIN
        -- Full attendance for most employees
        INSERT INTO ChamCong (MaNV, NgayLamViec, MaCa, GioVao, GioRa, TrangThai) VALUES
            (1, @ad, 1, '06:00', '14:00', N'Có mặt'),
            (2, @ad, 1, '06:05', '14:00', N'Có mặt'),
            (3, @ad, 2, '14:00', '22:00', N'Có mặt'),
            (5, @ad, 1, '06:00', '14:00', N'Có mặt'),
            (6, @ad, 3, '18:00', '23:00', N'Có mặt'),
            (7, @ad, 1, '06:00', '14:00', N'Có mặt'),
            (8, @ad, 3, '18:00', '23:00', N'Có mặt');

        -- MaNV=4 arrives late some days
        IF DAY(@ad) % 3 = 0
            INSERT INTO ChamCong (MaNV, NgayLamViec, MaCa, GioVao, GioRa, TrangThai)
            VALUES (4, @ad, 2, '14:30', '22:00', N'Trễ');
        ELSE IF DAY(@ad) % 5 = 0
            INSERT INTO ChamCong (MaNV, NgayLamViec, MaCa, GioVao, GioRa, TrangThai)
            VALUES (4, @ad, 2, NULL, NULL, N'Vắng');
        ELSE
            INSERT INTO ChamCong (MaNV, NgayLamViec, MaCa, GioVao, GioRa, TrangThai)
            VALUES (4, @ad, 2, '14:00', '22:00', N'Có mặt');

        SET @ad = DATEADD(DAY, 1, @ad);
    END
END
GO

-- 7d. Bonus/Penalty sample data for February 2026
IF NOT EXISTS (SELECT 1 FROM ThuongPhat WHERE Thang = 2 AND Nam = 2026)
BEGIN
    INSERT INTO ThuongPhat (MaNV, Thang, Nam, Loai, SoTien, LyDo) VALUES
    -- Bonuses
    (1, 2, 2026, N'Thưởng', 500000,  N'Thưởng bếp trưởng tháng xuất sắc'),
    (3, 2, 2026, N'Thưởng', 200000,  N'Thưởng phục vụ khách VIP'),
    (7, 2, 2026, N'Thưởng', 1000000, N'Thưởng quản lý — doanh thu tăng 15%'),
    -- Penalties
    (4, 2, 2026, N'Phạt',   100000,  N'Phạt đi trễ 3 lần trong tháng'),
    (4, 2, 2026, N'Phạt',   50000,   N'Phạt vắng không phép 1 ngày');
END
GO


-- =============================================
-- ═══════════════════════════════════════════
--  STORED PROCEDURES
-- ═══════════════════════════════════════════
-- =============================================


-- =============================================
-- SP 1: sp_TinhLuongThang
-- Monthly Payroll Generation
--
-- Generates/updates payroll for ALL active
-- employees for a given month/year.
--
-- Logic:
--   1. Count attendance days & hours from ChamCong
--   2. Calculate weighted shift multiplier
--   3. Compute salary = (Base / 26) × Days × Avg Multiplier
--   4. Add bonuses, subtract penalties
--   5. Deduct BHXH (10.5%), tax
--   6. MERGE into BangLuong (upsert)
--
-- Uses MERGE for atomic upsert — SQL Server 2019+
-- =============================================
IF OBJECT_ID('dbo.sp_TinhLuongThang', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_TinhLuongThang;
GO

CREATE PROCEDURE dbo.sp_TinhLuongThang
    @Thang      INT,
    @Nam        INT,
    @MaNV_Filter INT = NULL  -- NULL = all active employees
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- ── Validation ──
    IF @Thang NOT BETWEEN 1 AND 12
    BEGIN
        RAISERROR(N'Tháng phải từ 1 đến 12.', 16, 1);
        RETURN;
    END

    IF @Nam < 2020 OR @Nam > 2100
    BEGIN
        RAISERROR(N'Năm không hợp lệ.', 16, 1);
        RETURN;
    END

    -- ── Constants ──
    DECLARE @NgayCongChuan DECIMAL(5,1) = 26.0;
    DECLARE @TyLeBHXH     DECIMAL(5,4) = 0.1050;

    -- Date range for the month
    DECLARE @NgayDau DATE = DATEFROMPARTS(@Nam, @Thang, 1);
    DECLARE @NgayCuoi DATE = EOMONTH(@NgayDau);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- ══════════════════════════════════════════
        -- CTE 1: Attendance aggregation per employee
        -- ══════════════════════════════════════════
        ;WITH CTE_ChamCong AS (
            SELECT
                cc.MaNV,
                -- Count only actual presence days (Có mặt, Trễ, Về sớm count as worked)
                COUNT(DISTINCT CASE
                    WHEN cc.TrangThai IN (N'Có mặt', N'Trễ', N'Về sớm')
                    THEN cc.NgayLamViec
                END) AS SoNgayDiLam,
                -- Total hours worked
                ISNULL(SUM(cc.SoGioLam), 0) AS TongGioLam,
                -- Weighted-average shift multiplier
                ISNULL(
                    SUM(cc.SoGioLam * ISNULL(ca.HeSoLuong, 1.00))
                    / NULLIF(SUM(cc.SoGioLam), 0),
                    1.00
                ) AS HeSoTrungBinh
            FROM ChamCong cc
            LEFT JOIN CaLamViec ca ON cc.MaCa = ca.MaCa
            WHERE cc.NgayLamViec BETWEEN @NgayDau AND @NgayCuoi
              AND cc.TrangThai <> N'Vắng'
            GROUP BY cc.MaNV
        ),

        -- ══════════════════════════════════════════
        -- CTE 2: Bonus/Penalty aggregation
        -- ══════════════════════════════════════════
        CTE_ThuongPhat AS (
            SELECT
                MaNV,
                ISNULL(SUM(CASE WHEN Loai = N'Thưởng' THEN SoTien END), 0) AS TongThuong,
                ISNULL(SUM(CASE WHEN Loai = N'Phạt'   THEN SoTien END), 0) AS TongPhat
            FROM ThuongPhat
            WHERE Thang = @Thang AND Nam = @Nam
            GROUP BY MaNV
        ),

        -- ══════════════════════════════════════════
        -- CTE 3: Final payroll calculation
        -- ══════════════════════════════════════════
        CTE_TinhLuong AS (
            SELECT
                nv.MaNV,
                -- Attendance metrics
                ISNULL(cc.SoNgayDiLam, 0)    AS SoNgayChamCong,
                ISNULL(cc.TongGioLam, 0)     AS TongGioLam,
                ISNULL(cc.HeSoTrungBinh, 1)  AS HeSoTrungBinh,

                -- Salary calculation
                ROUND(
                    (nv.LuongCoBan / @NgayCongChuan)
                    * ISNULL(cc.SoNgayDiLam, 0)
                    * ISNULL(cc.HeSoTrungBinh, 1.00),
                    0
                ) AS LuongTheoCong,

                -- Deductions
                ROUND(nv.LuongCoBan * @TyLeBHXH, 0) AS BHXH,
                CAST(0 AS DECIMAL(18,0))             AS Thue,

                -- Bonus/Penalty
                ISNULL(tp.TongThuong, 0) AS TienThuong,
                ISNULL(tp.TongPhat, 0)   AS TienPhat

            FROM NhanVien nv
            LEFT JOIN CTE_ChamCong cc ON nv.MaNV = cc.MaNV
            LEFT JOIN CTE_ThuongPhat tp ON nv.MaNV = tp.MaNV
            WHERE nv.TrangThai = N'Đang làm'
              AND (@MaNV_Filter IS NULL OR nv.MaNV = @MaNV_Filter)
        )

        -- ══════════════════════════════════════════
        -- MERGE: Upsert into BangLuong
        -- ══════════════════════════════════════════
        MERGE BangLuong AS target
        USING (
            SELECT
                MaNV,
                @Thang AS Thang,
                @Nam   AS Nam,
                SoNgayChamCong                           AS NgayCongThucTe,
                LuongTheoCong,
                CAST(0 AS DECIMAL(18,0))                 AS TienUng,
                BHXH,
                Thue,
                -- Net pay = salary + bonus - penalty - bhxh - tax - advance
                -- Floor at 0
                CASE
                    WHEN (LuongTheoCong + TienThuong - TienPhat - BHXH - Thue) < 0
                    THEN 0
                    ELSE (LuongTheoCong + TienThuong - TienPhat - BHXH - Thue)
                END                                      AS TongThucNhan,
                SoNgayChamCong,
                TongGioLam,
                TienThuong,
                TienPhat,
                HeSoTrungBinh
            FROM CTE_TinhLuong
        ) AS source
        ON  target.MaNV  = source.MaNV
        AND target.Thang = source.Thang
        AND target.Nam   = source.Nam

        WHEN MATCHED THEN
            UPDATE SET
                NgayCongThucTe  = source.NgayCongThucTe,
                LuongTheoCong   = source.LuongTheoCong,
                -- Preserve manually-entered TienUng
                BHXH            = source.BHXH,
                Thue            = source.Thue,
                TongThucNhan    = CASE
                    WHEN (source.LuongTheoCong + source.TienThuong
                          - source.TienPhat - source.BHXH
                          - source.Thue - target.TienUng) < 0
                    THEN 0
                    ELSE (source.LuongTheoCong + source.TienThuong
                          - source.TienPhat - source.BHXH
                          - source.Thue - target.TienUng)
                END,
                SoNgayChamCong  = source.SoNgayChamCong,
                TongGioLam      = source.TongGioLam,
                TienThuong      = source.TienThuong,
                TienPhat        = source.TienPhat,
                HeSoTrungBinh   = source.HeSoTrungBinh,
                NgayCapNhat     = SYSDATETIME()

        WHEN NOT MATCHED THEN
            INSERT (MaNV, Thang, Nam, NgayCongThucTe, LuongTheoCong,
                    TienUng, BHXH, Thue, TongThucNhan,
                    SoNgayChamCong, TongGioLam,
                    TienThuong, TienPhat, HeSoTrungBinh, NgayCapNhat)
            VALUES (source.MaNV, source.Thang, source.Nam,
                    source.NgayCongThucTe, source.LuongTheoCong,
                    0, source.BHXH, source.Thue, source.TongThucNhan,
                    source.SoNgayChamCong, source.TongGioLam,
                    source.TienThuong, source.TienPhat,
                    source.HeSoTrungBinh, SYSDATETIME());

        PRINT N'Đã tính lương tháng ' + CAST(@Thang AS NVARCHAR)
            + N'/' + CAST(@Nam AS NVARCHAR)
            + N' cho ' + CAST(@@ROWCOUNT AS NVARCHAR) + N' nhân viên.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO


-- =============================================
-- SP 2: sp_BaoCaoChamCong
-- Attendance Summary Report
--
-- Returns per-employee attendance breakdown:
--   total days, present, late, absent, leave,
--   total hours, average hours/day
-- =============================================
IF OBJECT_ID('dbo.sp_BaoCaoChamCong', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_BaoCaoChamCong;
GO

CREATE PROCEDURE dbo.sp_BaoCaoChamCong
    @Thang INT,
    @Nam   INT,
    @MaNV  INT = NULL  -- NULL = all employees
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NgayDau  DATE = DATEFROMPARTS(@Nam, @Thang, 1);
    DECLARE @NgayCuoi DATE = EOMONTH(@NgayDau);

    SELECT
        nv.MaNV,
        nv.HoTen,
        bp.TenBoPhan,
        nv.ChucVu,

        -- Scheduled days (from LichLamViec)
        (SELECT COUNT(DISTINCT NgayLamViec)
         FROM LichLamViec
         WHERE MaNV = nv.MaNV
           AND NgayLamViec BETWEEN @NgayDau AND @NgayCuoi
        ) AS SoNgayXepLich,

        -- Actual attendance counts
        COUNT(DISTINCT CASE
            WHEN cc.TrangThai IN (N'Có mặt', N'Trễ', N'Về sớm')
            THEN cc.NgayLamViec
        END) AS SoNgayDiLam,

        COUNT(DISTINCT CASE
            WHEN cc.TrangThai = N'Có mặt' THEN cc.NgayLamViec
        END) AS SoNgayCoMat,

        COUNT(DISTINCT CASE
            WHEN cc.TrangThai = N'Trễ' THEN cc.NgayLamViec
        END) AS SoNgayTre,

        COUNT(DISTINCT CASE
            WHEN cc.TrangThai = N'Về sớm' THEN cc.NgayLamViec
        END) AS SoNgayVeSom,

        COUNT(DISTINCT CASE
            WHEN cc.TrangThai = N'Vắng' THEN cc.NgayLamViec
        END) AS SoNgayVang,

        COUNT(DISTINCT CASE
            WHEN cc.TrangThai = N'Nghỉ phép' THEN cc.NgayLamViec
        END) AS SoNgayNghiPhep,

        -- Hours
        ISNULL(SUM(cc.SoGioLam), 0)   AS TongGioLam,
        ISNULL(
            ROUND(
                SUM(cc.SoGioLam)
                / NULLIF(
                    COUNT(DISTINCT CASE
                        WHEN cc.TrangThai IN (N'Có mặt', N'Trễ', N'Về sớm')
                        THEN cc.NgayLamViec
                    END), 0
                ),
                2
            ), 0
        ) AS TrungBinhGio

    FROM NhanVien nv
    INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
    LEFT JOIN ChamCong cc
        ON  nv.MaNV = cc.MaNV
        AND cc.NgayLamViec BETWEEN @NgayDau AND @NgayCuoi
    WHERE nv.TrangThai = N'Đang làm'
      AND (@MaNV IS NULL OR nv.MaNV = @MaNV)
    GROUP BY nv.MaNV, nv.HoTen, bp.TenBoPhan, nv.ChucVu
    ORDER BY nv.HoTen;
END
GO


-- =============================================
-- SP 3: sp_TongHopThuongPhat
-- Bonus/Penalty Summary Report
--
-- Returns detailed bonus/penalty breakdown
-- per employee for a given month.
-- =============================================
IF OBJECT_ID('dbo.sp_TongHopThuongPhat', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_TongHopThuongPhat;
GO

CREATE PROCEDURE dbo.sp_TongHopThuongPhat
    @Thang INT,
    @Nam   INT,
    @MaNV  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Detail records
    SELECT
        tp.MaThuongPhat,
        tp.MaNV,
        nv.HoTen,
        bp.TenBoPhan,
        tp.Loai,
        tp.SoTien,
        tp.LyDo,
        tp.NgayTao
    FROM ThuongPhat tp
    INNER JOIN NhanVien nv ON tp.MaNV = nv.MaNV
    INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
    WHERE tp.Thang = @Thang
      AND tp.Nam   = @Nam
      AND (@MaNV IS NULL OR tp.MaNV = @MaNV)
    ORDER BY nv.HoTen, tp.Loai, tp.NgayTao;

    -- Summary aggregation
    SELECT
        nv.MaNV,
        nv.HoTen,
        ISNULL(SUM(CASE WHEN tp.Loai = N'Thưởng' THEN tp.SoTien END), 0) AS TongThuong,
        ISNULL(SUM(CASE WHEN tp.Loai = N'Phạt'   THEN tp.SoTien END), 0) AS TongPhat,
        ISNULL(SUM(CASE WHEN tp.Loai = N'Thưởng' THEN tp.SoTien END), 0)
        - ISNULL(SUM(CASE WHEN tp.Loai = N'Phạt' THEN tp.SoTien END), 0) AS ChenhLech
    FROM NhanVien nv
    LEFT JOIN ThuongPhat tp
        ON  nv.MaNV  = tp.MaNV
        AND tp.Thang = @Thang
        AND tp.Nam   = @Nam
    WHERE nv.TrangThai = N'Đang làm'
      AND (@MaNV IS NULL OR nv.MaNV = @MaNV)
    GROUP BY nv.MaNV, nv.HoTen
    HAVING SUM(tp.SoTien) IS NOT NULL
    ORDER BY nv.HoTen;
END
GO


-- =============================================
-- SP 4: sp_XemLichLamViec
-- Shift Schedule View
--
-- Returns the schedule for all employees
-- in a given date range with shift details.
-- =============================================
IF OBJECT_ID('dbo.sp_XemLichLamViec', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_XemLichLamViec;
GO

CREATE PROCEDURE dbo.sp_XemLichLamViec
    @TuNgay DATE,
    @DenNgay DATE,
    @MaNV    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ll.MaLich,
        ll.NgayLamViec,
        DATENAME(WEEKDAY, ll.NgayLamViec) AS ThuTrongTuan,
        nv.MaNV,
        nv.HoTen,
        bp.TenBoPhan,
        ca.TenCa,
        ca.GioBatDau,
        ca.GioKetThuc,
        ca.HeSoLuong,
        -- Check if there's an attendance record
        CASE
            WHEN cc.MaChamCong IS NOT NULL THEN cc.TrangThai
            ELSE N'Chưa chấm'
        END AS TrangThaiChamCong,
        ll.GhiChu
    FROM LichLamViec ll
    INNER JOIN NhanVien nv ON ll.MaNV = nv.MaNV
    INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
    INNER JOIN CaLamViec ca ON ll.MaCa = ca.MaCa
    LEFT JOIN ChamCong cc
        ON  ll.MaNV = cc.MaNV
        AND ll.MaCa = cc.MaCa
        AND ll.NgayLamViec = cc.NgayLamViec
    WHERE ll.NgayLamViec BETWEEN @TuNgay AND @DenNgay
      AND (@MaNV IS NULL OR ll.MaNV = @MaNV)
    ORDER BY ll.NgayLamViec, ca.GioBatDau, nv.HoTen;
END
GO


-- =============================================
-- SP 5: sp_ThongKeLuongChiTiet
-- Detailed Payroll Statistics
--
-- Returns the full payroll breakdown including
-- attendance, shift multipliers, and bonuses.
-- =============================================
IF OBJECT_ID('dbo.sp_ThongKeLuongChiTiet', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ThongKeLuongChiTiet;
GO

CREATE PROCEDURE dbo.sp_ThongKeLuongChiTiet
    @Thang INT,
    @Nam   INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bl.MaBangLuong,
        nv.MaNV,
        nv.HoTen,
        bp.TenBoPhan,
        nv.ChucVu,
        nv.LuongCoBan,

        -- Attendance from BangLuong snapshot
        bl.SoNgayChamCong,
        bl.TongGioLam,
        bl.HeSoTrungBinh,

        -- Salary breakdown
        bl.NgayCongThucTe,
        bl.LuongTheoCong,
        bl.TienThuong,
        bl.TienPhat,
        bl.TienUng,
        bl.BHXH,
        bl.Thue,
        bl.TongThucNhan,
        bl.NgayCapNhat,

        -- Attendance quality stats (live from ChamCong)
        (SELECT COUNT(*)
         FROM ChamCong cc
         WHERE cc.MaNV = nv.MaNV
           AND cc.NgayLamViec BETWEEN
               DATEFROMPARTS(@Nam, @Thang, 1)
               AND EOMONTH(DATEFROMPARTS(@Nam, @Thang, 1))
           AND cc.TrangThai = N'Trễ'
        ) AS SoLanTre,
        (SELECT COUNT(*)
         FROM ChamCong cc
         WHERE cc.MaNV = nv.MaNV
           AND cc.NgayLamViec BETWEEN
               DATEFROMPARTS(@Nam, @Thang, 1)
               AND EOMONTH(DATEFROMPARTS(@Nam, @Thang, 1))
           AND cc.TrangThai = N'Vắng'
        ) AS SoLanVang

    FROM BangLuong bl
    INNER JOIN NhanVien nv ON bl.MaNV = nv.MaNV
    INNER JOIN BoPhan bp ON nv.MaBoPhan = bp.MaBoPhan
    WHERE bl.Thang = @Thang
      AND bl.Nam   = @Nam
    ORDER BY nv.HoTen;
END
GO


PRINT N'';
PRINT N'═══════════════════════════════════════════════';
PRINT N'Migration 002 hoàn tất!';
PRINT N'   • 4 bảng mới: CaLamViec, LichLamViec,';
PRINT N'     ChamCong, ThuongPhat';
PRINT N'   • 6 cột mới trong BangLuong';
PRINT N'   • 4 index tối ưu';
PRINT N'   • 5 stored procedure';
PRINT N'   • Dữ liệu mẫu tháng 02/2026';
PRINT N'═══════════════════════════════════════════════';
GO
