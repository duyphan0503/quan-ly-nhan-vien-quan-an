# HƯỚNG DẪN TRIỂN KHAI & BÀN GIAO

**Quản Lý Nhân Viên Quán Ăn — Deployment & Handover Guide**

> Tài liệu này hướng dẫn chi tiết cách thiết lập môi trường phát triển trên Windows,
> cách triển khai ứng dụng lên máy chủ sản xuất, và cách đóng gói bộ cài đặt.

---

## Mục Lục

1. [Tổng Quan Kiến Trúc](#1-tổng-quan-kiến-trúc)
2. [Yêu Cầu Hệ Thống](#2-yêu-cầu-hệ-thống)
3. [Cấu Trúc Mã Nguồn](#3-cấu-trúc-mã-nguồn)
4. [Thiết Lập Môi Trường Phát Triển (Windows)](#4-thiết-lập-môi-trường-phát-triển-windows)
5. [Khởi Tạo Database](#5-khởi-tạo-database)
6. [Build & Chạy Ứng Dụng](#6-build--chạy-ứng-dụng)
7. [Triển Khai Sản Xuất](#7-triển-khai-sản-xuất)
8. [Đóng Gói Windows Installer](#8-đóng-gói-windows-installer)
9. [Bảo Trì & Vận Hành](#9-bảo-trì--vận-hành)
10. [Xử Lý Sự Cố](#10-xử-lý-sự-cố)
11. [Checklist Bàn Giao](#11-checklist-bàn-giao)

---

## 1. Tổng Quan Kiến Trúc

### Kiến Trúc Ứng Dụng

```
┌────────────────────────────────────────────────────────────┐
│                    WINDOWS (Dev hoặc Server)                │
│                                                            │
│  ┌────────────────────────┐    ┌─────────────────────────┐ │
│  │    QuanLyNhanVien.exe  │    │     SQL Server 2019+    │ │
│  │    .NET Framework 4.7.2│───▶│     QuanLyNhanVien DB   │ │
│  │    Windows Forms (C#)  │    │     Port 1433 (TCP/IP)  │ │
│  └────────────────────────┘    └─────────────────────────┘ │
│                                                            │
│  Layered Architecture:                                     │
│    Forms → Services → DataAccess (ADO.NET) → SQL Server    │
│    Infrastructure: AppLogger, ConnectionDiagnostics,       │
│                    GlobalExceptionHandler, SecurityHelper,  │
│                    ExcelExporter (ClosedXML)                │
└────────────────────────────────────────────────────────────┘
```

### Kiến Trúc Phân Tầng

| Tầng               | Thư mục          | Mô tả                                                       |
| :----------------- | :--------------- | :----------------------------------------------------------- |
| **Presentation**   | `Forms/`         | Giao diện WinForms (Login, Dashboard, CRUD, Thống kê)        |
| **Custom Controls**| `Controls/`      | RoundedButton, GlassPanel, DashboardCard                     |
| **Service**        | `Services/`      | Logic nghiệp vụ, validation, kết quả `ServiceResult<T>`     |
| **Data Access**    | `DataAccess/`    | ADO.NET thuần, `DatabaseHelper` quản lý connection           |
| **Infrastructure** | `Infrastructure/`| Logger, Exception Handler, Connection Diagnostics, Security, **ExcelExporter** |
| **Models**         | `Models/`        | Entity classes: TaiKhoan, BoPhan, NhanVien, BangLuong        |

### Luồng Khởi Động Ứng Dụng

```
Program.Main()
  │
  ├── 1. GlobalExceptionHandler.Install()     ← Bắt mọi exception chưa xử lý
  ├── 2. DatabaseHelper.TestConnection()      ← Kiểm tra kết nối DB
  │       │
  │       ├── ✅ Thành công → FormLogin
  │       └── ❌ Thất bại  → FormConnectionWizard (4 bước chẩn đoán)
  │                            │
  │                            ├── Bước 1: Nhập Server + Port
  │                            ├── Bước 2: Nhập Username + Password + Database
  │                            ├── Bước 3: Kiểm tra tự động (TCP → Auth → DB → Schema)
  │                            └── Bước 4: Lưu cấu hình vào App.config
  │
  └── 3. Application.Run(FormLogin)
```

---

## 2. Yêu Cầu Hệ Thống

### Máy Phát Triển (Developer)

| Yêu cầu           | Tối thiểu                                | Khuyến nghị                           |
| :----------------- | :--------------------------------------- | :------------------------------------ |
| **OS**             | Windows 10 (Build 1809+)                 | Windows 11                            |
| **IDE**            | Visual Studio 2022 Community             | Visual Studio 2022 Professional       |
| **Workload**       | .NET desktop development                 | —                                     |
| **SQL Server**     | SQL Server 2019 Developer / Express      | SQL Server 2022 Developer             |
| **SSMS**           | SQL Server Management Studio 19+         | SSMS 20                               |
| **.NET Framework** | 4.7.2 (có sẵn trên Windows 10)           | 4.8                                   |

### Máy Sản Xuất (Server)

| Yêu cầu           | Tối thiểu                | Khuyến nghị              |
| :----------------- | :----------------------- | :----------------------- |
| **OS**             | Windows Server 2019      | Windows Server 2022      |
| **CPU**            | 2 cores                  | 4 cores                  |
| **RAM**            | 4 GB                     | 8 GB                     |
| **Disk**           | 20 GB SSD                | 50 GB SSD                |
| **SQL Server**     | SQL Server 2019 Express  | SQL Server 2022 Standard |
| **.NET Framework** | 4.7.2                    | 4.8                      |

### Máy Trạm (Client)

| Yêu cầu           | Tối thiểu                                |
| :----------------- | :--------------------------------------- |
| **OS**             | Windows 10 (Build 1809+)                 |
| **.NET Framework** | 4.7.2 (đã có sẵn trên Windows 10)       |
| **RAM**            | 2 GB                                     |
| **Mạng**           | LAN kết nối được đến SQL Server          |

---

## 3. Cấu Trúc Mã Nguồn

```
quan-ly-nhan-vien-quan-an/
├── README.md                          # Giới thiệu dự án
├── HUONG_DAN_TRIEN_KHAI.md            # Tài liệu này
├── LICENSE                            # Giấy phép MIT
│
├── Deploy/                            # Script triển khai
│   ├── Init-Database.ps1              #   ★ Script tự động khởi tạo DB
│   └── Backup-Database.ps1            #   ★ Script backup DB tự động
│
├── docs/                              # Tài liệu đồ án
│
└── QuanLyNhanVien/                    # Source code chính
    ├── QuanLyNhanVien.sln             # Solution file (mở bằng Visual Studio)
    ├── QuanLyNhanVien.csproj          # Project file (.NET Framework 4.7.2)
    ├── App.config                     # Connection string configuration
    ├── Program.cs                     # Entry point
    ├── AppColors.cs                   # Bảng màu Catppuccin Mocha
    ├── AppFonts.cs                    # Font cross-platform (Segoe UI / fallback)
    ├── login.cfg                      # File lưu "Ghi nhớ đăng nhập" (mã hóa DPAPI)
    │
    ├── Controls/                      # Custom UI controls
    │   ├── RoundedButton.cs           #   Nút bo tròn với hiệu ứng hover
    │   ├── GlassPanel.cs              #   Panel hiệu ứng glassmorphism
    │   └── DashboardCard.cs           #   Card thống kê cho dashboard
    │
    ├── DataAccess/                    # Data Access Layer (ADO.NET)
    │   ├── DatabaseHelper.cs          #   Connection factory, TestConnection, RefreshConnectionString
    │   ├── TaiKhoanDAL.cs             #   CRUD tài khoản
    │   ├── BoPhanDAL.cs               #   CRUD bộ phận
    │   ├── NhanVienDAL.cs             #   CRUD nhân viên
    │   └── BangLuongDAL.cs            #   CRUD bảng lương
    │
    ├── Forms/                         # WinForms UI
    │   ├── FormLogin.cs               #   Đăng nhập (tích hợp AppLogger)
    │   ├── FormConnectionWizard.cs    #   ★ Wizard cấu hình kết nối DB (4 bước)
    │   ├── FormMain.cs                #   Trang chính / điều hướng
    │   ├── FormDashboard.cs           #   Dashboard tổng quan
    │   ├── FormNhanVien.cs            #   Quản lý nhân viên
    │   ├── FormBoPhan.cs              #   Quản lý bộ phận
    │   ├── FormBangLuong.cs           #   Quản lý bảng lương + Xuất Excel
    │   └── FormThongKe.cs             #   Thống kê / báo cáo
    │
    ├── Infrastructure/                # Hạ tầng dùng chung
    │   ├── ExcelExporter.cs           #   ★ Xuất phiếu lương Excel (ClosedXML)
    │   ├── AppLogger.cs               #   ★ Logger kép (File + Database)
    │   ├── GlobalExceptionHandler.cs  #   ★ Bắt exception toàn cục (UI + background thread)
    │   ├── ConnectionDiagnostics.cs   #   ★ Chẩn đoán TCP → Auth → DB → Schema
    │   ├── SecurityHelper.cs          #   Mã hóa/giải mã DPAPI (cho "Ghi nhớ mật khẩu")
    │   ├── LoginSettings.cs           #   Quản lý file login.cfg
    │   └── GridHelper.cs              #   Fix alignment cho DataGridView (hỗ trợ Mono)
    │
    ├── Models/                        # Domain models
    │   ├── TaiKhoan.cs                #   Tài khoản quản lý
    │   ├── BoPhan.cs                  #   Bộ phận / phòng ban
    │   ├── NhanVien.cs                #   Nhân viên
    │   └── BangLuong.cs               #   Bảng lương tháng
    │
    ├── Services/                      # Business logic layer
    │   ├── ServiceResult.cs           #   Wrapper Result<T> (không dùng exception)
    │   ├── TaiKhoanService.cs         #   Xác thực, CRUD tài khoản
    │   ├── NhanVienService.cs         #   Nghiệp vụ nhân viên
    │   ├── BoPhanService.cs           #   Nghiệp vụ bộ phận
    │   ├── BangLuongService.cs        #   Tính lương
    │   ├── ThongKeService.cs          #   Thống kê
    │   └── DashboardService.cs        #   Dữ liệu dashboard
    │
    └── SQL/                           # Database scripts (idempotent — chạy lại an toàn)
        ├── CreateDatabase.sql         #   001: Tạo DB + 4 bảng cốt lõi + dữ liệu mẫu
        ├── 002_ExpandSchema.sql       #   002: Ca làm, chấm công, thưởng/phạt, 5 stored procedures
        └── 003_ErrorLog.sql           #   003: Bảng ErrorLog + SP đọc/dọn dẹp nhật ký
```

### Database Schema

```
TaiKhoan                          (Tài khoản quản lý — admin/admin123)
    │
BoPhan ──────┐                    (Bộ phận: Bếp, Phục vụ, Thu ngân, Bảo vệ, Quản lý, Hải sản)
    │        │
NhanVien ────┘                    (Nhân viên → FK tới BoPhan)
    │
    ├── BangLuong                 (Lương theo tháng — snapshot đầy đủ)
    ├── CaLamViec                 (Định nghĩa ca: sáng, chiều, tối, khuya + hệ số lương)
    ├── LichLamViec               (Phân ca cho nhân viên theo ngày)
    ├── ChamCong                  (Chấm công: giờ vào/ra + tính SoGioLam tự động)
    ├── ThuongPhat                (Thưởng/Phạt theo tháng)
    └── ErrorLog                  (Nhật ký lỗi ứng dụng)
```

**Stored Procedures (tạo bởi `002_ExpandSchema.sql`):**

| Tên SP                    | Chức năng                                              |
| :------------------------ | :----------------------------------------------------- |
| `sp_TinhLuongThang`       | Tính lương tháng tự động (MERGE upsert)                |
| `sp_BaoCaoChamCong`       | Báo cáo chấm công (có mặt, trễ, vắng, nghỉ phép)     |
| `sp_TongHopThuongPhat`    | Tổng hợp thưởng/phạt theo tháng                       |
| `sp_XemLichLamViec`       | Xem lịch phân ca theo khoảng ngày                      |
| `sp_ThongKeLuongChiTiet`  | Thống kê lương chi tiết (kèm chấm công, ca làm)       |
| `sp_DocNhatKy`            | Đọc nhật ký lỗi (lọc theo mức độ, ngày)               |
| `sp_DonDepNhatKy`         | Dọn dẹp nhật ký cũ (giữ N ngày gần nhất)              |

---

## 4. Thiết Lập Môi Trường Phát Triển (Windows)

### 4.1. Cài Đặt Visual Studio 2022

1. Tải **Visual Studio 2022 Community** (miễn phí) tại: https://visualstudio.microsoft.com/
2. Khi cài đặt, chọn workload: **".NET desktop development"**
   - Workload này sẽ cài .NET Framework 4.7.2 SDK và MSBuild.

### 4.2. Cài Đặt SQL Server

Nếu chưa có SQL Server, tải và cài đặt một trong hai phiên bản:

- **SQL Server 2022 Developer** (miễn phí, đầy đủ tính năng): https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **SQL Server 2022 Express** (miễn phí, giới hạn 10GB): Cùng link trên

**Lưu ý khi cài đặt:**

| Cấu hình               | Giá trị cần chọn                              |
| :---------------------- | :--------------------------------------------- |
| Authentication Mode     | **Mixed Mode** (SQL Server + Windows Auth)     |
| SA Password             | Đặt mật khẩu mạnh, ghi nhớ lại               |
| Instance Name           | Default hoặc Named (ví dụ: `SQLDEV2022`)       |
| TCP/IP                  | **Phải bật** (xem mục 4.3)                    |

> [!IMPORTANT]
> **Named Instance:** Nếu bạn cài SQL Server dạng Named Instance (ví dụ: `SQLDEV2022`),
> thì Server Name trong SSMS sẽ là `TEN_MAY\SQLDEV2022` thay vì `localhost`.
> Và connection string trong App.config cần điều chỉnh tương ứng (xem mục 6.2).

### 4.3. Cấu Hình SQL Server Sau Cài Đặt

#### Bật TCP/IP Protocol

1. Mở **SQL Server Configuration Manager**
   - Trên Windows 10/11: Tìm kiếm `SQL Server 2022 Configuration Manager`
2. Vào **SQL Server Network Configuration** → **Protocols for [INSTANCE_NAME]**
3. Chuột phải **TCP/IP** → **Enable**
4. Restart dịch vụ SQL Server (xem bước dưới)

#### Đảm bảo SQL Server đang chạy

```powershell
# Xem tất cả dịch vụ SQL Server trên máy
Get-Service -Name "MSSQL*" | Select-Object Name, Status, DisplayName

# Khởi động dịch vụ (ví dụ named instance SQLDEV2022)
Start-Service -Name "MSSQL`$SQLDEV2022"

# Hoặc nếu dùng default instance
Start-Service -Name "MSSQLSERVER"
```

#### Bật tài khoản SA (Nếu chưa bật)

Mở SSMS, kết nối bằng Windows Authentication, rồi chạy:

```sql
ALTER LOGIN sa ENABLE;
ALTER LOGIN sa WITH PASSWORD = 'MatKhauCuaBan123!';
GO
```

---

## 5. Khởi Tạo Database

Có **2 cách** để khởi tạo database:

### Cách 1: Chạy Script SQL Thủ Công (Qua SSMS)

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server instance của bạn
3. Mở và chạy (Execute) từng file theo thứ tự:

| Thứ tự | File                    | Tác dụng                                                  |
| :----: | :---------------------- | :-------------------------------------------------------- |
|   1    | `CreateDatabase.sql`    | Tạo DB `QuanLyNhanVien`, 4 bảng cốt lõi, dữ liệu mẫu   |
|   2    | `002_ExpandSchema.sql`  | Thêm 4 bảng (ca, chấm công, thưởng/phạt), 5 stored proc  |
|   3    | `003_ErrorLog.sql`      | Tạo bảng ErrorLog + 2 stored proc đọc/dọn dẹp            |

Các file nằm tại: `QuanLyNhanVien/SQL/`

> [!TIP]
> Tất cả script đều **idempotent** — có thể chạy lại nhiều lần mà không bị lỗi duplicate.

### Cách 2: Chạy Script PowerShell Tự Động

```powershell
# Cài module SqlServer (nếu chưa có)
Install-Module -Name SqlServer -AllowClobber -Scope CurrentUser

# Di chuyển đến thư mục SQL
cd QuanLyNhanVien\SQL

# Chạy script tự động với mật khẩu SA của bạn
..\..\..\Deploy\Init-Database.ps1 -SqlPassword "MatKhauCuaBan123!"

# Nếu dùng named instance (ví dụ SQLDEV2022)
..\..\..\Deploy\Init-Database.ps1 -ServerInstance ".\SQLDEV2022" -SqlPassword "MatKhauCuaBan123!"

# Nếu file SQL nằm ở thư mục khác
..\..\..\Deploy\Init-Database.ps1 -ServerInstance "localhost" -SqlPassword "MyP@ss" -ScriptDirectory "C:\Path\To\SQL"
```

Script `Init-Database.ps1` sẽ tự động:
1. Kiểm tra module SqlServer/SQLPS
2. Kiểm tra file SQL có đủ không
3. Test kết nối TCP đến SQL Server
4. Test xác thực SQL
5. Chạy 3 file migration theo thứ tự
6. Xác minh kết quả (9 bảng, 7 stored proc, dữ liệu mẫu)

### Xác Minh Database Đã Tạo Thành Công

Chạy trong SSMS hoặc PowerShell:

```sql
USE QuanLyNhanVien;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME;
```

**Kết quả mong đợi — 9 bảng:**

```
BangLuong
BoPhan
CaLamViec
ChamCong
ErrorLog
LichLamViec
NhanVien
TaiKhoan
ThuongPhat
```

### Tài Khoản Mặc Định

Sau khi chạy `CreateDatabase.sql`, hệ thống có tài khoản admin:

| Trường            | Giá trị    |
| :---------------- | :--------- |
| **Tên đăng nhập** | `admin`    |
| **Mật khẩu**      | `admin123` |
| **Vai trò**       | `Admin`    |

> [!NOTE]
> **Mật khẩu `admin123` được lưu dạng SHA-256 hash trong database.**
> Chuỗi hash: `240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9`
> Trong môi trường sản xuất, hãy đổi mật khẩu ngay sau lần đăng nhập đầu tiên.

---

## 6. Build & Chạy Ứng Dụng

### 6.1. Mở Project

1. Mở file `QuanLyNhanVien/QuanLyNhanVien.sln` bằng **Visual Studio 2022**
2. Visual Studio sẽ tự động restore NuGet packages:
   - **ClosedXML** (v0.105.0) — Xuất phiếu lương Excel
   - **FontAwesome.Sharp** (v6.6.0) — Icon cho giao diện

> [!NOTE]
> Nếu NuGet không tự restore, right-click Solution → **Restore NuGet Packages**

### 6.2. Cấu Hình Connection String

Mở file `QuanLyNhanVien/App.config`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="QuanLyNhanVien"
         connectionString="Server=localhost,1433;Database=QuanLyNhanVien;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

**Sửa connection string cho phù hợp:**

| Tình huống                        | Giá trị `Server`                   |
| :-------------------------------- | :--------------------------------- |
| Default instance trên localhost   | `localhost,1433`                   |
| Named instance (ví dụ SQLDEV2022) | `localhost\SQLDEV2022` hoặc `.\SQLDEV2022` |
| Máy chủ qua mạng LAN             | `192.168.1.100,1433`               |
| Named instance trên máy khác     | `192.168.1.100\SQLEXPRESS`         |

> [!NOTE]
> **Không cần sửa App.config thủ công!** Nếu connection string sai, khi chạy ứng dụng sẽ
> **tự động hiện Connection Wizard** để bạn nhập thông tin và tự lưu vào config.

### 6.3. Build & Run

1. Chọn cấu hình: **Debug** (để phát triển) hoặc **Release** (để triển khai)
2. Nhấn **F5** (Debug) hoặc **Ctrl+F5** (Run without debugging)
3. Nếu database kết nối thành công → hiển thị **FormLogin**
4. Nếu database không kết nối được → hiển thị **Connection Wizard**

### 6.4. Connection Wizard (Lần Chạy Đầu)

Khi ứng dụng không kết nối được DB, wizard tự động hiện ra:

1. **Bước 1 — Server:** Nhập tên máy chủ (ví dụ: `.\SQLDEV2022`) và port (`1433`)
2. **Bước 2 — Credentials:** Nhập tên đăng nhập SQL (`sa`), mật khẩu, tên database (`QuanLyNhanVien`)
3. **Bước 3 — Kiểm Tra:** Wizard chạy 4 bước chẩn đoán tự động:
   - ✅ **TCP Connectivity** — Ping server:port
   - ✅ **SQL Authentication** — Đăng nhập thử vào `master`
   - ✅ **Database Existence** — Kiểm tra DB `QuanLyNhanVien` tồn tại
   - ✅ **Schema Verification** — Kiểm tra 4 bảng cốt lõi (TaiKhoan, BoPhan, NhanVien, BangLuong)
4. **Bước 4 — Lưu:** Nhấn "LƯU CẤU HÌNH" → cập nhật `App.config` → hiển thị FormLogin

---

## 7. Triển Khai Sản Xuất

### 7.1. Chuẩn Bị Máy Chủ Windows

#### Kiểm tra .NET Framework

```powershell
Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\' |
    Get-ItemPropertyValue -Name Release |
    ForEach-Object {
        if ($_ -ge 461808) { Write-Host "✅ .NET Framework 4.7.2+ đã cài đặt (Release: $_)" }
        else { Write-Host "❌ Cần cài .NET Framework 4.7.2 (Release hiện tại: $_)" }
    }
```

#### Mở Tường Lửa Cho SQL Server

```powershell
# Mở port 1433 TCP cho SQL Server
New-NetFirewallRule -DisplayName "SQL Server Port 1433" `
    -Direction Inbound -LocalPort 1433 -Protocol TCP -Action Allow

# Mở port cho SQL Server Browser (cần thiết cho named instances)
New-NetFirewallRule -DisplayName "SQL Server Browser" `
    -Direction Inbound -LocalPort 1434 -Protocol UDP -Action Allow
```

#### Tạo Thư Mục Ứng Dụng

```powershell
$appRoot = "C:\QuanLyNhanVien"
New-Item -ItemType Directory -Force -Path "$appRoot\App"
New-Item -ItemType Directory -Force -Path "$appRoot\SQL"
New-Item -ItemType Directory -Force -Path "$appRoot\Logs"
New-Item -ItemType Directory -Force -Path "$appRoot\Backup"
```

### 7.2. Cài Đặt SQL Server Trên Máy Chủ

Cài **SQL Server 2022 Express** (miễn phí): https://www.microsoft.com/en-us/sql-server/sql-server-downloads

Sau khi cài, đảm bảo:
1. **TCP/IP**: Đã bật (SQL Server Configuration Manager)
2. **Mixed Authentication Mode**: Đã bật
3. **SA Account**: Đã enable và đặt mật khẩu mạnh
4. **Service đang chạy**: `Get-Service MSSQLSERVER`

### 7.3. Khởi Tạo Database Trên Máy Chủ

```powershell
# Copy file SQL và script
Copy-Item ".\QuanLyNhanVien\SQL\*" "C:\QuanLyNhanVien\SQL\" -Recurse
Copy-Item ".\Deploy\Init-Database.ps1" "C:\QuanLyNhanVien\SQL\"

# Chạy khởi tạo
cd C:\QuanLyNhanVien\SQL
.\Init-Database.ps1 -SqlPassword "MatKhau_San_Xuat_Manh!@#456"

# Hoặc với named instance
.\Init-Database.ps1 -ServerInstance ".\SQLEXPRESS" -SqlPassword "MatKhau_San_Xuat_Manh!@#456"
```

### 7.4. Build Bản Release & Copy

```powershell
# Build Release trong Visual Studio:
# Build → Configuration Manager → Release → Build Solution
# Hoặc dùng MSBuild từ command line:
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
    QuanLyNhanVien.csproj /t:Rebuild /p:Configuration=Release

# Copy file cần thiết
$sourceDir = ".\QuanLyNhanVien\bin\Release"
$targetDir = "C:\QuanLyNhanVien\App"

Copy-Item "$sourceDir\QuanLyNhanVien.exe" $targetDir -Force
Copy-Item "$sourceDir\QuanLyNhanVien.exe.config" $targetDir -Force
Copy-Item "$sourceDir\QuanLyNhanVien.pdb" $targetDir -Force  # Optional: cho debug

# Copy các DLL thư viện (ClosedXML, FontAwesome...)
Copy-Item "$sourceDir\*.dll" $targetDir -Force
```

### 7.5. Cập Nhật Connection String Cho Máy Chủ

Sửa `C:\QuanLyNhanVien\App\QuanLyNhanVien.exe.config`:

```xml
<connectionStrings>
    <add name="QuanLyNhanVien"
         connectionString="Server=192.168.1.100,1433;Database=QuanLyNhanVien;User Id=sa;Password=MatKhau_San_Xuat_Manh!@#456;TrustServerCertificate=True"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

> [!NOTE]
> Hoặc bỏ qua bước này — chạy ứng dụng lần đầu, **Connection Wizard** sẽ tự hiện
> và hướng dẫn nhân viên IT cấu hình kết nối.

---

## 8. Đóng Gói Windows Installer

### Phương án A: Inno Setup (.exe) — Đơn Giản & Hiệu Quả

Tải [Inno Setup 6.x](https://jrsoftware.org/isinfo.php), tạo file `Installer/Setup.iss`:

```iss
; Inno Setup Script — Quản Lý Nhân Viên Quán Ăn

[Setup]
AppName=Quản Lý Nhân Viên Quán Ăn
AppVersion=1.0.0
AppPublisher=Restaurant Management
DefaultDirName={autopf}\QuanLyNhanVien
DefaultGroupName=Quản Lý Nhân Viên
AllowNoIcons=yes
OutputDir=Output
OutputBaseFilename=QuanLyNhanVien-Setup-v1.0.0
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x86 x64 arm64
ArchitecturesInstallIn64BitMode=x64 arm64

[Tasks]
Name: "desktopicon"; Description: "Tạo biểu tượng trên Desktop"; GroupDescription: "Biểu tượng:"

[Files]
Source: "..\bin\Release\QuanLyNhanVien.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\QuanLyNhanVien.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\QuanLyNhanVien.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\SQL\CreateDatabase.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion
Source: "..\SQL\002_ExpandSchema.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion
Source: "..\SQL\003_ErrorLog.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion
Source: "..\..\Deploy\Init-Database.ps1"; DestDir: "{app}\SQL"; Flags: ignoreversion

[Dirs]
Name: "{app}\Logs"; Permissions: everyone-modify

[Icons]
Name: "{group}\Quản Lý Nhân Viên"; Filename: "{app}\QuanLyNhanVien.exe"
Name: "{group}\Gỡ Cài Đặt"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Quản Lý Nhân Viên"; Filename: "{app}\QuanLyNhanVien.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\QuanLyNhanVien.exe"; Description: "Khởi chạy Quản Lý Nhân Viên"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNetInstalled(): Boolean;
var
  releaseValue: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', releaseValue) then
    Result := (releaseValue >= 461808); // .NET Framework 4.7.2
end;

function InitializeSetup(): Boolean;
begin
  if not IsDotNetInstalled() then
  begin
    MsgBox('Yêu cầu .NET Framework 4.7.2 trở lên.' + #13#10 +
           'Vui lòng tải và cài đặt tại:' + #13#10 +
           'https://dotnet.microsoft.com/download/dotnet-framework/net472', mbError, MB_OK);
    Result := False;
    Exit;
  end;
  Result := True;
end;
```

Build installer:

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Installer\Setup.iss
# Output: Installer\Output\QuanLyNhanVien-Setup-v1.0.0.exe
```

### Phương án B: WiX Toolset (.msi) — Cho Triển Khai Doanh Nghiệp

Nếu cần file `.msi` chuẩn cho Group Policy deployment, xem [WiX Toolset Documentation](https://wixtoolset.org/docs/wix3/).

---

## 9. Bảo Trì & Vận Hành

### 9.1. Backup Database

**Chạy bằng script có sẵn:**

```powershell
# Backup một lần
.\Deploy\Backup-Database.ps1 -SqlPassword "MatKhau123!"

# Backup với named instance
.\Deploy\Backup-Database.ps1 -ServerInstance ".\SQLEXPRESS" -SqlPassword "MatKhau123!"

# Backup vào thư mục tùy chọn, giữ 60 ngày
.\Deploy\Backup-Database.ps1 -SqlPassword "MatKhau123!" `
    -BackupDirectory "D:\Backups\QLNV" -RetentionDays 60
```

**Tự động hóa với Task Scheduler:**

```powershell
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
    -Argument "-File C:\QuanLyNhanVien\SQL\Backup-Database.ps1 -SqlPassword 'MatKhau123!'"
$trigger = New-ScheduledTaskTrigger -Daily -At "02:00AM"
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -RunLevel Highest

Register-ScheduledTask -TaskName "QuanLyNhanVien-DailyBackup" `
    -Action $action -Trigger $trigger -Principal $principal `
    -Description "Backup CSDL Quản Lý Nhân Viên hàng ngày"
```

### 9.2. Giám Sát Lỗi

Ứng dụng có **2 kênh ghi log**:

1. **File log** (luôn hoạt động): `[thư_mục_exe]/Logs/yyyy-MM-dd.log`
2. **Database log** (best-effort): Bảng `ErrorLog`

```powershell
# Xem file log mới nhất
$latestLog = Get-ChildItem "C:\QuanLyNhanVien\App\Logs\" |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($latestLog) { Get-Content $latestLog.FullName -Tail 50 }

# Xem 20 lỗi gần nhất từ ErrorLog trong DB
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_DocNhatKy @SoLuong = 20, @MucDo = N'Error'"
```

### 9.3. Dọn Dẹp Log

```powershell
# Dọn dẹp ErrorLog trong DB (giữ 90 ngày gần nhất)
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_DonDepNhatKy @SoNgayGiu = 90"

# Dọn dẹp file log cũ (giữ 30 file gần nhất)
Get-ChildItem "C:\QuanLyNhanVien\App\Logs\*.log" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -Skip 30 |
    Remove-Item -Force
```

### 9.4. Cập Nhật Phiên Bản Mới

```powershell
# 1. Backup trước khi cập nhật
$timestamp = Get-Date -Format "yyyyMMdd"
Copy-Item "C:\QuanLyNhanVien\App" "C:\QuanLyNhanVien\Backup\App_$timestamp" -Recurse

# 2. Copy file mới (GIỮ NGUYÊN file .config!)
Copy-Item ".\bin\Release\QuanLyNhanVien.exe" "C:\QuanLyNhanVien\App\" -Force
Copy-Item ".\bin\Release\QuanLyNhanVien.pdb" "C:\QuanLyNhanVien\App\" -Force
# ⚠️ KHÔNG copy QuanLyNhanVien.exe.config — để giữ connection string hiện tại

# 3. Chạy migration SQL mới (nếu có)
# Invoke-Sqlcmd -ServerInstance ... -InputFile "new_migration.sql"

# 4. Khởi động lại ứng dụng
Start-Process "C:\QuanLyNhanVien\App\QuanLyNhanVien.exe"
```

---

## 10. Xử Lý Sự Cố

### 10.1. SQL Server Không Khởi Động Được — "Access is Denied" (OS Error 5)

**Triệu chứng:** Dịch vụ SQL Server có trạng thái **Stopped**, không thể Start.

**Nguyên nhân phổ biến:** Tài khoản dịch vụ `NT Service\MSSQL$<INSTANCE>` không có quyền truy cập vào thư mục chứa file dữ liệu (`master.mdf`, `mastlog.ldf`).

**Cách kiểm tra:**

```powershell
# Xem đường dẫn file dữ liệu
Get-WmiObject win32_service |
    Where-Object { $_.Name -like "MSSQL*" } |
    Select-Object Name, PathName

# Hoặc xem Error Log của SQL Server (nếu có file)
# Thường ở: [SQL_INSTALL_PATH]\MSSQL\Log\ERRORLOG
```

**Cách sửa (chạy PowerShell với quyền Administrator):**

```powershell
# Thay <INSTANCE> bằng tên instance của bạn (ví dụ: SQLDEV2022)
# Thay <PATH> bằng đường dẫn thực tế đến thư mục DATA

# Cấp quyền cho thư mục dữ liệu
icacls "<PATH>\MSSQL\DATA" /grant "NT Service\MSSQL`$<INSTANCE>:(OI)(CI)F" /T

# Cấp quyền cho thư mục log
icacls "<PATH>\MSSQL\Log" /grant "NT Service\MSSQL`$<INSTANCE>:(OI)(CI)F" /T

# Khởi động lại
Start-Service -Name "MSSQL`$<INSTANCE>"
```

**Ví dụ cụ thể:**

```powershell
icacls "P:\SQL2022\MSSQL16.SQLDEV2022\MSSQL\DATA" /grant "NT Service\MSSQL`$SQLDEV2022:(OI)(CI)F" /T
icacls "P:\SQL2022\MSSQL16.SQLDEV2022\MSSQL\Log" /grant "NT Service\MSSQL`$SQLDEV2022:(OI)(CI)F" /T
Start-Service -Name "MSSQL`$SQLDEV2022"
```

### 10.2. Ứng Dụng Không Kết Nối Được Đến SQL Server

**Triệu chứng:** Connection Wizard hiện ra mỗi lần mở app, hoặc lỗi "Lỗi kết nối CSDL".

**Kiểm tra theo thứ tự:**

|  #  | Kiểm tra                    | Lệnh kiểm tra                                    | Cách sửa                              |
| :-: | :-------------------------- | :------------------------------------------------ | :------------------------------------ |
|  1  | SQL Server dịch vụ có chạy? | `Get-Service -Name "MSSQL*"`                      | `Start-Service -Name "MSSQL$..."`     |
|  2  | Port 1433 mở?               | `Test-NetConnection localhost -Port 1433`          | Bật TCP/IP trong SQL Config Manager   |
|  3  | Tường lửa chặn?             | `Test-NetConnection <IP> -Port 1433`               | Mở firewall (xem mục 7.1)            |
|  4  | SA account bật?             | SSMS → Security → Logins → sa                     | `ALTER LOGIN sa ENABLE`               |
|  5  | Mixed Auth mode?            | SSMS → Server Properties → Security               | Chọn "SQL Server and Windows Auth"    |
|  6  | Database tồn tại?           | SSMS → Databases                                  | Chạy `Init-Database.ps1` (mục 5)     |

### 10.3. Lỗi "Đăng Nhập Thất Bại" (Error 18456)

```powershell
# Kiểm tra authentication mode
Invoke-Sqlcmd -ServerInstance "localhost" -TrustServerCertificate `
    -Query "SELECT SERVERPROPERTY('IsIntegratedSecurityOnly') AS 'WindowsAuthOnly'"
# Nếu = 1 → Chỉ có Windows Auth → Cần bật Mixed Mode

# Bật Mixed Mode (cần kết nối bằng Windows Authentication trước)
Invoke-Sqlcmd -ServerInstance "localhost" -TrustServerCertificate `
    -Query "EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE',
            N'Software\Microsoft\MSSQLServer\MSSQLServer', N'LoginMode', REG_DWORD, 2"

# Restart SQL Server sau khi thay đổi
Restart-Service MSSQLSERVER -Force
# Hoặc cho named instance:
Restart-Service "MSSQL`$SQLDEV2022" -Force
```

### 10.4. Ứng Dụng Crash Không Hiển Thị Lỗi

**Bước 1:** Kiểm tra file log (AppLogger luôn ghi vào file, kể cả khi DB chưa kết nối):

```powershell
# File log nằm cạnh file .exe, trong thư mục Logs/
Get-ChildItem "[đường_dẫn_exe]\Logs\" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 |
    ForEach-Object { Get-Content $_.FullName -Tail 100 }
```

**Bước 2:** Kiểm tra Windows Event Log:

```powershell
Get-WinEvent -FilterHashtable @{
    LogName = 'Application'
    Level = 2  # Error
    ProviderName = '.NET Runtime'
} -MaxEvents 10 | Format-List TimeCreated, Message
```

### 10.5. Named Instance — Lưu Ý Quan Trọng

Khi dùng Named Instance (ví dụ: `SQLDEV2022`), cần lưu ý:

| Hạng mục              | Default Instance   | Named Instance            |
| :--------------------- | :----------------- | :------------------------ |
| Kết nối SSMS           | `localhost`        | `TEN_MAY\SQLDEV2022`      |
| Service name           | `MSSQLSERVER`      | `MSSQL$SQLDEV2022`        |
| Connection string      | `Server=localhost,1433` | `Server=.\SQLDEV2022` |
| SQL Browser            | Không cần          | **Phải bật** để client tìm được instance |

Bật SQL Server Browser:

```powershell
Set-Service -Name "SQLBrowser" -StartupType Automatic
Start-Service -Name "SQLBrowser"
```

---

## 11. Checklist Bàn Giao

### Cho Đội IT Triển Khai

- [ ] **Máy chủ:** Windows Server đã cài đặt và cập nhật
- [ ] **.NET Framework:** Phiên bản 4.7.2+ đã xác nhận
- [ ] **SQL Server:** Đã cài đặt, TCP/IP bật, Mixed Auth mode
- [ ] **SA Account:** Đã enable, mật khẩu mạnh đã đặt
- [ ] **SQL Server Service:** Đang chạy (không bị lỗi Access Denied)
- [ ] **Firewall:** Port 1433 TCP đã mở (nếu truy cập qua mạng)
- [ ] **Database:** Đã chạy `Init-Database.ps1` thành công
- [ ] **Xác nhận bảng:** 9 bảng + 7 stored proc đã tạo
- [ ] **Ứng dụng:** Đã deploy vào thư mục đích
- [ ] **Connection string:** Đã cấu hình đúng (hoặc Wizard đã lưu)
- [ ] **Đăng nhập thử:** admin / admin123 → thành công
- [ ] **Log hoạt động:** File log được tạo trong `Logs/`
- [ ] **Backup:** Scheduled task đã tạo (`Deploy/Backup-Database.ps1`)

### Cho Đội Phát Triển

- [ ] **Source code:** Đã push lên repository
- [ ] **SQL scripts:** 3 file migration đều idempotent
- [ ] **Build:** Release build thành công (0 errors, 0 warnings)
- [ ] **Connection Wizard:** Hoạt động đúng 4 bước chẩn đoán
- [ ] **Installer:** Inno Setup/MSI đã tạo và test
- [ ] **Tài liệu:** `HUONG_DAN_TRIEN_KHAI.md` đã cập nhật
- [ ] **Password mặc định:** Đã thay thế trong môi trường sản xuất

### Sản Phẩm Bàn Giao

|  #  | Hạng mục         | File/Thư mục                                               |
| :-: | :--------------- | :--------------------------------------------------------- |
|  1  | Source Code      | Toàn bộ thư mục `QuanLyNhanVien/`                          |
|  2  | SQL Scripts      | `SQL/CreateDatabase.sql`, `SQL/002_*.sql`, `SQL/003_*.sql` |
|  3  | Deploy Scripts   | `Deploy/Init-Database.ps1`, `Deploy/Backup-Database.ps1`   |
|  4  | Installer        | `Installer/Output/QuanLyNhanVien-Setup-v1.0.0.exe`         |
|  5  | Tài liệu         | `README.md` + `HUONG_DAN_TRIEN_KHAI.md` + `BI_KIP_GIAI_TRINH.md` |

---

> **Tài liệu này được cập nhật lần cuối: 2026-02-22**
>
> Phiên bản ứng dụng: **1.0.0**
>
> Liên hệ hỗ trợ: _(điền thông tin liên hệ của đội phát triển)_
