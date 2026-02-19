-- =============================================
-- QUẢN LÝ NHÂN VIÊN QUÁN ĂN
-- Migration 003: Error Logging Table
--
-- Target: SQL Server 2019+
-- Date:   2026-02-14
-- =============================================

USE QuanLyNhanVien;
GO

SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- ErrorLog — Centralized application error log
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ErrorLog' AND xtype = 'U')
BEGIN
    CREATE TABLE ErrorLog (
        LogId       BIGINT IDENTITY(1,1) PRIMARY KEY,
        ThoiGian    DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
        MucDo       NVARCHAR(20)  NOT NULL DEFAULT N'Error',
            -- 'Info' | 'Warning' | 'Error' | 'Critical'
        NguonLoi    NVARCHAR(200) NOT NULL,
            -- Class/method that raised the error
        ThongBao    NVARCHAR(MAX) NOT NULL,
            -- Error message
        ChiTiet     NVARCHAR(MAX) NULL,
            -- Stack trace or additional details
        NguoiDung   NVARCHAR(50)  NULL,
            -- Currently logged-in user (if known)
        TenMay      NVARCHAR(100) NULL,
            -- Machine name
        PhienBan    NVARCHAR(20)  NULL,
            -- Application version

        CONSTRAINT CK_ErrorLog_MucDo
            CHECK (MucDo IN (N'Info', N'Warning', N'Error', N'Critical'))
    );
END
GO

-- Index for time-based queries and cleanup
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ErrorLog_ThoiGian'
      AND object_id = OBJECT_ID('ErrorLog')
)
    CREATE NONCLUSTERED INDEX IX_ErrorLog_ThoiGian
        ON ErrorLog (ThoiGian DESC)
        INCLUDE (MucDo, NguonLoi);
GO

-- Index for severity-based filtering
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ErrorLog_MucDo'
      AND object_id = OBJECT_ID('ErrorLog')
)
    CREATE NONCLUSTERED INDEX IX_ErrorLog_MucDo
        ON ErrorLog (MucDo, ThoiGian DESC);
GO

-- =============================================
-- SP: sp_DocNhatKy — Read recent log entries
-- =============================================
IF OBJECT_ID('dbo.sp_DocNhatKy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DocNhatKy;
GO

CREATE PROCEDURE dbo.sp_DocNhatKy
    @SoLuong    INT = 100,
    @MucDo      NVARCHAR(20) = NULL,
    @TuNgay     DATETIME2 = NULL,
    @DenNgay    DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (@SoLuong)
        LogId, ThoiGian, MucDo, NguonLoi,
        ThongBao, ChiTiet, NguoiDung, TenMay
    FROM ErrorLog
    WHERE (@MucDo IS NULL OR MucDo = @MucDo)
      AND (@TuNgay IS NULL OR ThoiGian >= @TuNgay)
      AND (@DenNgay IS NULL OR ThoiGian <= @DenNgay)
    ORDER BY ThoiGian DESC;
END
GO

-- =============================================
-- SP: sp_DonDepNhatKy — Cleanup old log entries
-- Keeps last N days, deletes the rest.
-- =============================================
IF OBJECT_ID('dbo.sp_DonDepNhatKy', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_DonDepNhatKy;
GO

CREATE PROCEDURE dbo.sp_DonDepNhatKy
    @SoNgayGiu INT = 90
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NgayCat DATETIME2 = DATEADD(DAY, -@SoNgayGiu, SYSDATETIME());

    DELETE FROM ErrorLog WHERE ThoiGian < @NgayCat;

    PRINT N'✅ Đã dọn dẹp nhật ký cũ hơn '
        + CAST(@SoNgayGiu AS NVARCHAR) + N' ngày. '
        + CAST(@@ROWCOUNT AS NVARCHAR) + N' bản ghi đã xóa.';
END
GO

PRINT N'✅ Migration 003: ErrorLog table created.';
GO
