-- =============================================
-- HỆ THỐNG QUẢN LÝ NHÂN VIÊN QUÁN ĂN
-- Script tạo cơ sở dữ liệu
-- =============================================

-- Tạo Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyNhanVien')
BEGIN
    CREATE DATABASE QuanLyNhanVien;
END
GO

USE QuanLyNhanVien;
GO

-- =============================================
-- 1. Bảng TaiKhoan — Tài khoản quản lý (Admin)
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'TaiKhoan' AND xtype = 'U')
BEGIN
    CREATE TABLE TaiKhoan (
        MaTK        INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
        MatKhau     NVARCHAR(256) NOT NULL,
        VaiTro      NVARCHAR(20) NOT NULL DEFAULT N'Admin'
    );
END
GO

-- =============================================
-- 2. Bảng BoPhan — Bộ phận / Chức vụ
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'BoPhan' AND xtype = 'U')
BEGIN
    CREATE TABLE BoPhan (
        MaBoPhan    INT IDENTITY(1,1) PRIMARY KEY,
        TenBoPhan   NVARCHAR(100) NOT NULL UNIQUE
    );
END
GO

-- =============================================
-- 3. Bảng NhanVien — Thông tin nhân viên
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'NhanVien' AND xtype = 'U')
BEGIN
    CREATE TABLE NhanVien (
        MaNV        INT IDENTITY(1,1) PRIMARY KEY,
        HoTen       NVARCHAR(100) NOT NULL,
        ChucVu      NVARCHAR(50),
        MaBoPhan    INT NOT NULL,
        LuongCoBan  DECIMAL(18,0) NOT NULL DEFAULT 0,
        TrangThai   NVARCHAR(20) NOT NULL DEFAULT N'Đang làm',
        CONSTRAINT FK_NhanVien_BoPhan FOREIGN KEY (MaBoPhan) REFERENCES BoPhan(MaBoPhan)
    );
END
GO

-- =============================================
-- 4. Bảng BangLuong — Lương theo tháng
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'BangLuong' AND xtype = 'U')
BEGIN
    CREATE TABLE BangLuong (
        MaBangLuong     INT IDENTITY(1,1) PRIMARY KEY,
        MaNV            INT NOT NULL,
        Thang           INT NOT NULL CHECK (Thang BETWEEN 1 AND 12),
        Nam             INT NOT NULL CHECK (Nam >= 2020),
        NgayCongThucTe  DECIMAL(5,1) NOT NULL DEFAULT 0,
        LuongTheoCong   DECIMAL(18,0) NOT NULL DEFAULT 0,
        TienUng         DECIMAL(18,0) NOT NULL DEFAULT 0,
        BHXH            DECIMAL(18,0) NOT NULL DEFAULT 0,
        Thue            DECIMAL(18,0) NOT NULL DEFAULT 0,
        TongThucNhan    DECIMAL(18,0) NOT NULL DEFAULT 0,
        CONSTRAINT FK_BangLuong_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
        CONSTRAINT UQ_BangLuong_NV_Thang_Nam UNIQUE (MaNV, Thang, Nam)
    );
END
GO

-- =============================================
-- DỮ LIỆU MẪU
-- =============================================

-- Tài khoản mặc định: admin / admin123 (mật khẩu lưu dạng SHA-256 hash)
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
VALUES (N'admin', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', N'Admin');
GO

-- Bộ phận mẫu
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Bếp');
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Phục vụ');
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Thu ngân');
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Bảo vệ');
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Quản lý');
INSERT INTO BoPhan (TenBoPhan) VALUES (N'Hải sản');
GO

-- Nhân viên mẫu
INSERT INTO NhanVien (HoTen, ChucVu, MaBoPhan, LuongCoBan, TrangThai) VALUES
(N'Nguyễn Văn An',    N'Bếp trưởng',     1, 8000000,  N'Đang làm'),
(N'Trần Thị Bình',    N'Phụ bếp',        1, 5500000,  N'Đang làm'),
(N'Lê Hoàng Cường',   N'Phục vụ',        2, 5000000,  N'Đang làm'),
(N'Phạm Minh Dũng',   N'Phục vụ',        2, 5000000,  N'Đang làm'),
(N'Hoàng Thị Lan',    N'Thu ngân',       3, 6000000,  N'Đang làm'),
(N'Võ Quốc Hùng',     N'Bảo vệ',        4, 4500000,  N'Đang làm'),
(N'Đặng Thị Mai',     N'Quản lý',        5, 10000000, N'Đang làm'),
(N'Ngô Thanh Tùng',   N'Chế biến HHS',   6, 6500000,  N'Đang làm'),
(N'Bùi Văn Phúc',     N'Phục vụ',        2, 5000000,  N'Nghỉ việc'),
(N'Lý Thị Hồng',      N'Phụ bếp',        1, 5500000,  N'Đang làm');
GO

-- Bảng lương mẫu (Tháng 1/2026)
INSERT INTO BangLuong (MaNV, Thang, Nam, NgayCongThucTe, LuongTheoCong, TienUng, BHXH, Thue, TongThucNhan)
VALUES
(1, 1, 2026, 23.5, 7230769, 1000000, 840000, 0, 5390769),
(2, 1, 2026, 22.0, 4653846, 500000,  577500, 0, 3576346),
(3, 1, 2026, 24.0, 4615385, 0,       525000, 0, 4090385),
(4, 1, 2026, 20.0, 3846154, 0,       525000, 0, 3321154),
(5, 1, 2026, 26.0, 6000000, 0,       630000, 0, 5370000),
(6, 1, 2026, 25.0, 4326923, 0,       472500, 0, 3854423),
(7, 1, 2026, 26.0, 10000000,2000000, 1050000,0, 6950000),
(8, 1, 2026, 23.0, 5750000, 0,       682500, 0, 5067500);
GO

PRINT N'Tạo cơ sở dữ liệu QuanLyNhanVien thành công!';
GO
