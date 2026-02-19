# HƯỚNG DẪN TRIỂN KHAI & BÀN GIAO

**Quản Lý Nhân Viên Quán Ăn — Deployment & Handover Guide**

> Tài liệu này hướng dẫn chi tiết quy trình triển khai ứng dụng từ môi trường phát triển (Docker trên Linux)
> sang môi trường sản xuất (Windows Server + SQL Server native). Bao gồm script tự động hóa và quy trình đóng gói cài đặt chuyên nghiệp.

---

## Mục Lục

1. [Tổng Quan Kiến Trúc](#1-tổng-quan-kiến-trúc)
2. [Yêu Cầu Hệ Thống](#2-yêu-cầu-hệ-thống)
3. [Cấu Trúc Mã Nguồn](#3-cấu-trúc-mã-nguồn)
4. [Giai Đoạn 1: Chuẩn Bị Máy Chủ Windows](#4-giai-đoạn-1-chuẩn-bị-máy-chủ-windows)
5. [Giai Đoạn 2: Cài Đặt SQL Server](#5-giai-đoạn-2-cài-đặt-sql-server)
6. [Giai Đoạn 3: Khởi Tạo Database (Tự Động)](#6-giai-đoạn-3-khởi-tạo-database-tự-động)
7. [Giai Đoạn 4: Triển Khai Ứng Dụng](#7-giai-đoạn-4-triển-khai-ứng-dụng)
8. [Giai Đoạn 5: Đóng Gói Windows Installer](#8-giai-đoạn-5-đóng-gói-windows-installer)
9. [Cấu Hình Sau Cài Đặt](#9-cấu-hình-sau-cài-đặt)
10. [Bảo Trì & Vận Hành](#10-bảo-trì--vận-hành)
11. [Xử Lý Sự Cố](#11-xử-lý-sự-cố)
12. [Checklist Bàn Giao](#12-checklist-bàn-giao)

---

## 1. Tổng Quan Kiến Trúc

### Môi Trường Phát Triển (Development)

```
┌────────────────────────────────────────────────────┐
│                LINUX WORKSTATION                    │
│                                                    │
│  ┌──────────────────┐    ┌───────────────────────┐ │
│  │   WinForms App   │    │ Docker Container      │ │
│  │   (Mono Runtime) │───▶│ SQL Server 2022 Dev   │ │
│  │   localhost:5000  │    │ localhost:1433         │ │
│  └──────────────────┘    └───────────────────────┘ │
│                                                    │
│  Build: msbuild / Mono 6.x                         │
│  DB Init: docker exec + sqlcmd                     │
└────────────────────────────────────────────────────┘
```

### Môi Trường Sản Xuất (Production)

```
┌────────────────────────────────────────────────────────────────┐
│                    WINDOWS SERVER                               │
│                                                                │
│  ┌────────────────────────┐    ┌─────────────────────────────┐ │
│  │    QuanLyNhanVien.exe  │    │     SQL Server 2019+        │ │
│  │    .NET Framework 4.7.2│───▶│     QuanLyNhanVien DB       │ │
│  │    (Native Windows)    │    │     Port 1433 (TCP/IP)      │ │
│  └────────────────────────┘    └─────────────────────────────┘ │
│                                                                │
│  ┌─────────────────────────────────┐                           │
│  │        Client Machines          │                           │
│  │  Win 10/11 + .NET 4.7.2        │──▶ SQL Server (LAN)       │
│  │  QuanLyNhanVien.exe             │                           │
│  └─────────────────────────────────┘                           │
└────────────────────────────────────────────────────────────────┘
```

### Khác Biệt Chính: Dev → Production

| Hạng mục       | Phát triển (Docker/Linux)            | Sản xuất (Windows)                  |
| :------------- | :----------------------------------- | :---------------------------------- |
| **SQL Server** | Docker container (Developer Edition) | Native install (Express / Standard) |
| **Runtime**    | Mono 6.x                             | .NET Framework 4.7.2                |
| **Connection** | `Server=localhost,1433`              | `Server=<SERVER_IP>,1433`           |
| **Build**      | `msbuild` (Mono)                     | Visual Studio / MSBuild             |
| **DB Init**    | `docker exec sqlcmd`                 | `Invoke-Sqlcmd` (PowerShell)        |
| **Password**   | Hardcoded `YourPassword123!`         | **Phải thay đổi!**                  |

> [!CAUTION]
> **MẬT KHẨU MẶC ĐỊNH `YourPassword123!` CHỈ DÙNG CHO MÔI TRƯỜNG PHÁT TRIỂN.**
> Khi triển khai sản xuất, PHẢI thay đổi mật khẩu SQL Server thành mật khẩu mạnh.

---

## 2. Yêu Cầu Hệ Thống

### Máy Chủ (Server)

| Yêu cầu            | Tối thiểu               | Khuyến nghị              |
| :----------------- | :---------------------- | :----------------------- |
| **OS**             | Windows Server 2019     | Windows Server 2022      |
| **CPU**            | 2 cores                 | 4 cores                  |
| **RAM**            | 4 GB                    | 8 GB                     |
| **Disk**           | 20 GB SSD               | 50 GB SSD                |
| **SQL Server**     | SQL Server 2019 Express | SQL Server 2022 Standard |
| **.NET Framework** | 4.7.2                   | 4.8                      |

### Máy Trạm (Client)

| Yêu cầu            | Tối thiểu                       |
| :----------------- | :------------------------------ |
| **OS**             | Windows 10 (Build 1809+)        |
| **.NET Framework** | 4.7.2 (đã có sẵn trên Win 10)   |
| **RAM**            | 2 GB                            |
| **Mạng**           | LAN kết nối được đến SQL Server |

### Phần Mềm Cần Cài Trước (Trên Máy Chủ)

1. **SQL Server 2019+ Express** — [Tải tại đây](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
2. **SQL Server Management Studio (SSMS)** — [Tải tại đây](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
3. **.NET Framework 4.7.2 Runtime** — [Tải tại đây](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) _(thường đã có sẵn)_

---

## 3. Cấu Trúc Mã Nguồn

```
QuanLyNhanVien/
├── Program.cs                     # Entry point — GlobalExceptionHandler → DB test → Wizard → Login
├── App.config                     # Connection string configuration
├── AppColors.cs                   # Catppuccin Mocha design tokens
├── AppFonts.cs                    # Cross-platform font detection
├── QuanLyNhanVien.csproj          # MSBuild project file
├── QuanLyNhanVien.sln             # Visual Studio solution
│
├── Controls/                      # Custom UI controls
│   ├── RoundedButton.cs           #   Rounded button with hover effects
│   ├── GlassPanel.cs              #   Glassmorphism panel
│   └── DashboardCard.cs           #   Statistic card widget
│
├── DataAccess/                    # Data Access Layer (ADO.NET)
│   ├── DatabaseHelper.cs          #   Connection factory + TestConnection + RefreshConnectionString
│   ├── TaiKhoanDAL.cs             #   Account CRUD
│   ├── BoPhanDAL.cs               #   Department CRUD
│   ├── NhanVienDAL.cs             #   Employee CRUD
│   └── BangLuongDAL.cs            #   Payroll CRUD
│
├── Forms/                         # WinForms UI
│   ├── FormLogin.cs               #   Login (with AppLogger integration)
│   ├── FormConnectionWizard.cs    #   ★ Database Connection Wizard (4-step diagnostic)
│   ├── FormMain.cs                #   Main dashboard / navigation
│   ├── FormNhanVien.cs            #   Employee management
│   ├── FormBoPhan.cs              #   Department management
│   ├── FormBangLuong.cs           #   Payroll management
│   └── FormThongKe.cs             #   Statistics / reports
│
├── Infrastructure/                # Cross-cutting concerns
│   ├── AppLogger.cs               #   ★ Dual-output logger (file + DB)
│   ├── GlobalExceptionHandler.cs  #   ★ Thread + AppDomain exception handlers
│   └── ConnectionDiagnostics.cs   #   ★ TCP → Auth → DB → Schema diagnostics
│
├── Models/                        # Domain models
│   ├── TaiKhoan.cs
│   ├── BoPhan.cs
│   ├── NhanVien.cs
│   └── BangLuong.cs
│
├── Services/                      # Business logic layer
│   ├── ServiceResult.cs           #   Result<T> wrapper (no exceptions for expected failures)
│   ├── TaiKhoanService.cs
│   ├── NhanVienService.cs
│   ├── BoPhanService.cs
│   ├── BangLuongService.cs
│   ├── ThongKeService.cs
│   └── DashboardService.cs
│
└── SQL/                           # Database scripts (idempotent, ordered)
    ├── CreateDatabase.sql          #   001: Core schema (TaiKhoan, BoPhan, NhanVien, BangLuong)
    ├── 002_ExpandSchema.sql        #   002: Shifts, attendance, bonus/penalty, stored procedures
    └── 003_ErrorLog.sql            #   003: ErrorLog table + cleanup procedures
```

### Database Schema (ER Summary)

```
TaiKhoan ─────────── (Admin accounts)
    │
BoPhan ──────┐       (Departments: Bếp, Phục vụ, Thu ngân...)
    │        │
NhanVien ────┘       (Employees → FK to BoPhan)
    │
    ├── BangLuong    (Monthly payroll records)
    ├── CaLamViec    (Shift definitions with salary multipliers)
    ├── LichLamViec  (Employee ↔ Shift schedule assignments)
    ├── ChamCong     (Attendance logs: check-in/out + computed hours)
    ├── ThuongPhat   (Bonus/penalty records)
    └── ErrorLog     (Application error logging)
```

---

## 4. Giai Đoạn 1: Chuẩn Bị Máy Chủ Windows

### 4.1. Kiểm Tra .NET Framework

Mở **PowerShell** với quyền Administrator:

```powershell
# Kiểm tra phiên bản .NET Framework đã cài đặt
Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\' |
    Get-ItemPropertyValue -Name Release |
    ForEach-Object {
        if ($_ -ge 461808) { Write-Host "✅ .NET Framework 4.7.2+ đã cài đặt (Release: $_)" }
        else { Write-Host "❌ Cần cài .NET Framework 4.7.2 (Release hiện tại: $_)" }
    }
```

Nếu chưa có, tải và cài đặt:

```powershell
# Tải .NET Framework 4.7.2 Offline Installer
Invoke-WebRequest -Uri "https://go.microsoft.com/fwlink/?LinkId=863265" -OutFile "$env:TEMP\ndp472-devpack.exe"
Start-Process "$env:TEMP\ndp472-devpack.exe" -ArgumentList "/passive /norestart" -Wait
```

### 4.2. Mở Tường Lửa Cho SQL Server

```powershell
# Mở port 1433 TCP cho SQL Server
New-NetFirewallRule -DisplayName "SQL Server Port 1433" `
    -Direction Inbound -LocalPort 1433 -Protocol TCP -Action Allow

# Mở port cho SQL Server Browser (cần thiết cho named instances)
New-NetFirewallRule -DisplayName "SQL Server Browser" `
    -Direction Inbound -LocalPort 1434 -Protocol UDP -Action Allow

Write-Host "✅ Firewall rules đã được thêm."
```

### 4.3. Tạo Thư Mục Ứng Dụng

```powershell
# Tạo cấu trúc thư mục chuẩn
$appRoot = "C:\QuanLyNhanVien"
New-Item -ItemType Directory -Force -Path "$appRoot\App"
New-Item -ItemType Directory -Force -Path "$appRoot\SQL"
New-Item -ItemType Directory -Force -Path "$appRoot\Logs"
New-Item -ItemType Directory -Force -Path "$appRoot\Backup"

Write-Host "✅ Cấu trúc thư mục:"
Get-ChildItem $appRoot -Directory | ForEach-Object { Write-Host "   📁 $_" }
```

---

## 5. Giai Đoạn 2: Cài Đặt SQL Server

### 5.1. SQL Server Express — Cài Đặt Nhanh

Nếu chưa có SQL Server, tải **SQL Server 2022 Express** (miễn phí):

```powershell
# Tải SQL Server Express
$sqlUrl = "https://go.microsoft.com/fwlink/p/?linkid=2216019&clcid=0x409&culture=en-us&country=us"
$setupPath = "$env:TEMP\SQLServer2022-SSEI-Expr.exe"
Invoke-WebRequest -Uri $sqlUrl -OutFile $setupPath
Write-Host "✅ Đã tải SQL Server Express. Chạy: $setupPath"
# Mở installer (cài đặt GUI)
Start-Process $setupPath -Wait
```

### 5.2. Cấu Hình Sau Cài Đặt

Sau khi cài SQL Server, cần đảm bảo:

1. **Bật TCP/IP Protocol:**

```powershell
# Import SQL Server module
Import-Module SQLPS -DisableNameChecking

# Bật TCP/IP
$tcp = Get-Item "SQLSERVER:\SQL\localhost\DEFAULT\ServerProtocols\TCP"
if ($tcp.IsEnabled -eq $false) {
    $tcp.IsEnabled = $true
    $tcp.Alter()
    Write-Host "✅ TCP/IP đã được bật. Cần restart SQL Server."
}
```

2. **Bật SQL Server Authentication Mode:**

```powershell
# Chạy trong SSMS hoặc sqlcmd:
# ALTER LOGIN sa ENABLE;
# ALTER LOGIN sa WITH PASSWORD = 'MatKhauMoi_Manh123!';
# GO
```

3. **Restart SQL Server Service:**

```powershell
Restart-Service -Name "MSSQLSERVER" -Force
Write-Host "✅ SQL Server đã restart."
```

> [!TIP]
> **Named Instance?** Nếu cài SQL Server dạng Named Instance (ví dụ: `SQLEXPRESS`),
> thay `localhost` bằng `localhost\SQLEXPRESS` trong connection string và bật SQL Server Browser service.

---

## 6. Giai Đoạn 3: Khởi Tạo Database (Tự Động)

### 6.1. Script PowerShell Tự Động

Lưu file sau tại `C:\QuanLyNhanVien\SQL\Init-Database.ps1`:

> **File này đã được cung cấp sẵn trong dự án — xem mục 6.2 bên dưới.**

### 6.2. Sử Dụng Script

```powershell
# Bước 1: Copy thư mục SQL từ source code sang máy chủ
Copy-Item -Path ".\QuanLyNhanVien\SQL\*" -Destination "C:\QuanLyNhanVien\SQL\" -Recurse

# Bước 2: Copy script khởi tạo
Copy-Item -Path ".\Deploy\Init-Database.ps1" -Destination "C:\QuanLyNhanVien\SQL\"

# Bước 3: Chạy với mật khẩu mới (KHÔNG dùng mật khẩu mặc định!)
cd C:\QuanLyNhanVien\SQL
.\Init-Database.ps1 -SqlPassword "MatKhau_Moi_Manh!@#456"

# Hoặc với named instance:
.\Init-Database.ps1 -ServerInstance ".\SQLEXPRESS" -SqlPassword "MatKhau_Moi_Manh!@#456"
```

### 6.3. Xác Minh Sau Khởi Tạo

```powershell
# Kiểm tra bảng đã tạo
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -Username "sa" -Password "MatKhau_Moi_Manh!@#456" `
    -TrustServerCertificate `
    -Query "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME"

# Kết quả mong đợi: 8 bảng
# BangLuong, BoPhan, CaLamViec, ChamCong,
# ErrorLog, LichLamViec, NhanVien, TaiKhoan, ThuongPhat
```

---

## 7. Giai Đoạn 4: Triển Khai Ứng Dụng

### 7.1. Build Bản Release

**Trên máy phát triển (hoặc CI/CD):**

```bash
# Build Release trên Linux/Mono
msbuild QuanLyNhanVien/QuanLyNhanVien.csproj /t:Rebuild /p:Configuration=Release

# Hoặc trên Windows với Visual Studio
# Mở QuanLyNhanVien.sln → Build → Configuration: Release → Build Solution
```

**Trên Windows (nếu build trên máy đích):**

```powershell
# Sử dụng MSBuild từ Visual Studio
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" `
    QuanLyNhanVien.csproj /t:Rebuild /p:Configuration=Release
```

### 7.2. Copy File Sang Máy Chủ

```powershell
# Danh sách file cần deploy
$sourceDir = ".\QuanLyNhanVien\bin\Release"
$targetDir = "C:\QuanLyNhanVien\App"

# Copy tất cả file cần thiết
$requiredFiles = @(
    "QuanLyNhanVien.exe",
    "QuanLyNhanVien.exe.config",
    "QuanLyNhanVien.pdb"          # Optional: cho debug trong production
)

foreach ($file in $requiredFiles) {
    $src = Join-Path $sourceDir $file
    if (Test-Path $src) {
        Copy-Item $src $targetDir -Force
        Write-Host "✅ Copied: $file"
    } else {
        Write-Host "⚠️ Not found: $file"
    }
}
```

### 7.3. Cập Nhật Connection String

Mở `C:\QuanLyNhanVien\App\QuanLyNhanVien.exe.config` và sửa:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <!-- ⚠️ THAY ĐỔI CÁC GIÁ TRỊ DƯỚI ĐÂY CHO MÔI TRƯỜNG SẢN XUẤT -->
    <add name="QuanLyNhanVien"
         connectionString="Server=192.168.1.100,1433;Database=QuanLyNhanVien;User Id=sa;Password=MatKhau_Moi_Manh!@#456;TrustServerCertificate=True"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

> [!NOTE]
> Nếu chạy lần đầu mà connection string sai, ứng dụng sẽ **tự động hiển thị Connection Wizard**
> để hướng dẫn nhân viên IT cấu hình kết nối đúng.

### 7.4. Kiểm Tra Hoạt Động

```powershell
# Chạy thử ứng dụng
Start-Process "C:\QuanLyNhanVien\App\QuanLyNhanVien.exe"

# Kiểm tra log file được tạo
Get-ChildItem "C:\QuanLyNhanVien\App\Logs\" -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 5 Name, LastWriteTime, Length

# Nếu có lỗi, xem log mới nhất
$latestLog = Get-ChildItem "C:\QuanLyNhanVien\App\Logs\" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1
if ($latestLog) {
    Get-Content $latestLog.FullName -Tail 50
}
```

---

## 8. Giai Đoạn 5: Đóng Gói Windows Installer

### Phương án A: WiX Toolset (.msi) — Chuyên nghiệp

WiX Toolset (Windows Installer XML) tạo file `.msi` chuẩn Windows, hỗ trợ:

- Cài/gỡ cài đặt qua Control Panel
- Group Policy deployment
- Silent installation cho IT triển khai hàng loạt

#### A.1. Cài Đặt WiX Toolset

```powershell
# Cài WiX qua Chocolatey
choco install wixtoolset -y

# Hoặc tải trực tiếp: https://wixtoolset.org/docs/wix3/
```

#### A.2. Tạo File WiX Project

Tạo file `Installer/QuanLyNhanVien.wxs`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">

  <Product Id="*"
           Name="Quản Lý Nhân Viên Quán Ăn"
           Language="1066"
           Version="1.0.0.0"
           Manufacturer="Restaurant Management"
           UpgradeCode="A1B2C3D4-E5F6-7890-ABCD-000000000001">

    <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine"
             Description="Hệ thống quản lý nhân viên cho quán ăn"
             Comments="Phần mềm quản lý nhân viên, tính lương, chấm công" />

    <!-- Upgrade handling — auto-remove previous versions -->
    <MajorUpgrade DowngradeErrorMessage="Phiên bản mới hơn đã được cài đặt." />
    <MediaTemplate EmbedCab="yes" />

    <!-- .NET Framework 4.7.2 prerequisite check -->
    <PropertyRef Id="WIX_IS_NETFRAMEWORK_472_OR_LATER_INSTALLED" />
    <Condition Message="Yêu cầu .NET Framework 4.7.2 trở lên. Vui lòng cài đặt trước.">
      <![CDATA[Installed OR WIX_IS_NETFRAMEWORK_472_OR_LATER_INSTALLED]]>
    </Condition>

    <!-- Features -->
    <Feature Id="MainApplication" Title="Ứng Dụng Chính" Level="1">
      <ComponentGroupRef Id="ApplicationFiles" />
      <ComponentRef Id="ApplicationShortcut" />
      <ComponentRef Id="DesktopShortcut" />
    </Feature>

    <Feature Id="SQLScripts" Title="Script Cơ Sở Dữ Liệu" Level="1">
      <ComponentGroupRef Id="SqlFiles" />
    </Feature>

    <!-- Install directory structure -->
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="QuanLyNhanVien">
          <Directory Id="SQLFolder" Name="SQL" />
          <Directory Id="LogsFolder" Name="Logs" />
        </Directory>
      </Directory>

      <!-- Start Menu -->
      <Directory Id="ProgramMenuFolder">
        <Directory Id="ApplicationProgramsFolder" Name="Quản Lý Nhân Viên" />
      </Directory>

      <!-- Desktop -->
      <Directory Id="DesktopFolder" Name="Desktop" />
    </Directory>

    <!-- Start Menu shortcut -->
    <DirectoryRef Id="ApplicationProgramsFolder">
      <Component Id="ApplicationShortcut" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000002">
        <Shortcut Id="AppStartMenuShortcut"
                  Name="Quản Lý Nhân Viên"
                  Description="Quản Lý Nhân Viên Quán Ăn"
                  Target="[INSTALLFOLDER]QuanLyNhanVien.exe"
                  WorkingDirectory="INSTALLFOLDER"
                  Icon="AppIcon.ico" />
        <RemoveFolder Id="CleanUpShortCut" Directory="ApplicationProgramsFolder" On="uninstall" />
        <RegistryValue Root="HKCU" Key="Software\QuanLyNhanVien"
                       Name="installed" Type="integer" Value="1" KeyPath="yes" />
      </Component>
    </DirectoryRef>

    <!-- Desktop shortcut -->
    <DirectoryRef Id="DesktopFolder">
      <Component Id="DesktopShortcut" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000003">
        <Shortcut Id="AppDesktopShortcut"
                  Name="Quản Lý Nhân Viên"
                  Description="Hệ thống quản lý nhân viên quán ăn"
                  Target="[INSTALLFOLDER]QuanLyNhanVien.exe"
                  WorkingDirectory="INSTALLFOLDER"
                  Icon="AppIcon.ico" />
        <RegistryValue Root="HKCU" Key="Software\QuanLyNhanVien"
                       Name="desktopShortcut" Type="integer" Value="1" KeyPath="yes" />
      </Component>
    </DirectoryRef>

    <!-- Application icon -->
    <Icon Id="AppIcon.ico" SourceFile="Resources\app.ico" />
    <Property Id="ARPPRODUCTICON" Value="AppIcon.ico" />

  </Product>

  <!-- Application files -->
  <Fragment>
    <ComponentGroup Id="ApplicationFiles" Directory="INSTALLFOLDER">
      <Component Id="MainExe" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000010">
        <File Id="QuanLyNhanVienExe" Source="..\bin\Release\QuanLyNhanVien.exe" KeyPath="yes" />
      </Component>
      <Component Id="MainConfig" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000011">
        <File Id="QuanLyNhanVienConfig" Source="..\bin\Release\QuanLyNhanVien.exe.config" KeyPath="yes" />
      </Component>
    </ComponentGroup>
  </Fragment>

  <!-- SQL script files -->
  <Fragment>
    <ComponentGroup Id="SqlFiles" Directory="SQLFolder">
      <Component Id="SqlCreate" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000020">
        <File Id="CreateDatabaseSql" Source="..\SQL\CreateDatabase.sql" KeyPath="yes" />
      </Component>
      <Component Id="SqlExpand" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000021">
        <File Id="ExpandSchemaSql" Source="..\SQL\002_ExpandSchema.sql" KeyPath="yes" />
      </Component>
      <Component Id="SqlErrorLog" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000022">
        <File Id="ErrorLogSql" Source="..\SQL\003_ErrorLog.sql" KeyPath="yes" />
      </Component>
      <Component Id="SqlInitPs" Guid="A1B2C3D4-E5F6-7890-ABCD-000000000023">
        <File Id="InitDatabasePs1" Source="..\..\Deploy\Init-Database.ps1" KeyPath="yes" />
      </Component>
    </ComponentGroup>
  </Fragment>

</Wix>
```

#### A.3. Build MSI

```powershell
cd Installer

# Compile WiX source
candle.exe QuanLyNhanVien.wxs -ext WixNetFxExtension
# Link into MSI
light.exe QuanLyNhanVien.wixobj -ext WixNetFxExtension -ext WixUIExtension -o QuanLyNhanVien-Setup.msi

Write-Host "✅ MSI created: QuanLyNhanVien-Setup.msi"
```

#### A.4. Silent Installation (Cho IT deploy hàng loạt)

```powershell
# Cài đặt silent (không cần GUI)
msiexec /i QuanLyNhanVien-Setup.msi /qn /l*v install.log

# Gỡ cài đặt silent
msiexec /x QuanLyNhanVien-Setup.msi /qn
```

---

### Phương án B: Inno Setup (.exe) — Đơn Giản Hơn

Nếu không cần MSI (ví dụ: triển khai quy mô nhỏ), dùng [Inno Setup](https://jrsoftware.org/isinfo.php).

#### B.1. Tạo File Inno Setup Script

Tạo file `Installer/Setup.iss`:

```iss
; Inno Setup Script — Quản Lý Nhân Viên Quán Ăn
; Compile with Inno Setup 6.x

[Setup]
AppName=Quản Lý Nhân Viên Quán Ăn
AppVersion=1.0.0
AppPublisher=Restaurant Management
AppPublisherURL=https://github.com/your-repo
DefaultDirName={autopf}\QuanLyNhanVien
DefaultGroupName=Quản Lý Nhân Viên
AllowNoIcons=yes
; Output settings
OutputDir=Output
OutputBaseFilename=QuanLyNhanVien-Setup-v1.0.0
; Compression
Compression=lzma2/ultra64
SolidCompression=yes
; Style
WizardStyle=modern
SetupIconFile=..\Resources\app.ico
UninstallDisplayIcon={app}\QuanLyNhanVien.exe
; Privileges
PrivilegesRequired=admin
; Architecture
ArchitecturesAllowed=x86 x64 arm64
ArchitecturesInstallIn64BitMode=x64 arm64

[Languages]
Name: "vietnamese"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Tạo biểu tượng trên Desktop"; GroupDescription: "Biểu tượng:"
Name: "quicklaunchicon"; Description: "Tạo biểu tượng Quick Launch"; GroupDescription: "Biểu tượng:"; Flags: unchecked

[Files]
; Application
Source: "..\bin\Release\QuanLyNhanVien.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\QuanLyNhanVien.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\Release\QuanLyNhanVien.pdb"; DestDir: "{app}"; Flags: ignoreversion

; SQL Scripts
Source: "..\SQL\CreateDatabase.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion
Source: "..\SQL\002_ExpandSchema.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion
Source: "..\SQL\003_ErrorLog.sql"; DestDir: "{app}\SQL"; Flags: ignoreversion

; PowerShell init script
Source: "..\..\Deploy\Init-Database.ps1"; DestDir: "{app}\SQL"; Flags: ignoreversion

; Create Logs directory
[Dirs]
Name: "{app}\Logs"; Permissions: everyone-modify

[Icons]
Name: "{group}\Quản Lý Nhân Viên"; Filename: "{app}\QuanLyNhanVien.exe"
Name: "{group}\Gỡ Cài Đặt"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Quản Lý Nhân Viên"; Filename: "{app}\QuanLyNhanVien.exe"; Tasks: desktopicon

[Run]
; Launch after install
Filename: "{app}\QuanLyNhanVien.exe"; Description: "Khởi chạy Quản Lý Nhân Viên"; Flags: nowait postinstall skipifsilent

[Code]
// Check .NET Framework version
function IsDotNetInstalled(): Boolean;
var
  releaseValue: Cardinal;
begin
  Result := False;
  if RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', releaseValue) then
  begin
    // 461808 = .NET Framework 4.7.2
    Result := (releaseValue >= 461808);
  end;
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

#### B.2. Build EXE Installer

```powershell
# Compile with Inno Setup Compiler
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" Installer\Setup.iss

# Output: Installer/Output/QuanLyNhanVien-Setup-v1.0.0.exe
```

---

## 9. Cấu Hình Sau Cài Đặt

### 9.1. Lần Chạy Đầu Tiên

Khi ứng dụng được chạy lần đầu (hoặc khi connection string không hợp lệ):

1. **Connection Wizard tự động hiện ra** — Không cần cấu hình thủ công!
2. Nhân viên IT nhập:
   - **Server:** IP hoặc tên máy chủ SQL Server (ví dụ: `192.168.1.100`)
   - **Port:** `1433` (mặc định)
   - **Username:** `sa`
   - **Password:** Mật khẩu SQL
   - **Database:** `QuanLyNhanVien`
3. Wizard chạy **4 bước kiểm tra tự động:**
   - ✅ TCP Connectivity (ping server:port)
   - ✅ SQL Authentication (đăng nhập thử)
   - ✅ Database Existence (kiểm tra DB có tồn tại)
   - ✅ Schema Verification (kiểm tra 4 bảng cốt lõi)
4. Nếu tất cả pass → **Lưu cấu hình → Hiển thị FormLogin**

### 9.2. Cấu Hình Mạng Client → Server

Trên **mỗi máy trạm** (client):

```
Ứng dụng → Connection Wizard → Nhập IP máy chủ → Test → Lưu → Đăng nhập
```

Cấu hình được lưu vào `QuanLyNhanVien.exe.config` ngay cạnh file `.exe`,
mỗi máy có thể có connection string khác nhau nếu cần.

### 9.3. Tài Khoản Mặc Định

Sau khi chạy `CreateDatabase.sql`, hệ thống có tài khoản admin mặc định:

| Trường            | Giá trị                                                      |
| :---------------- | :----------------------------------------------------------- |
| **Tên đăng nhập** | `admin`                                                      |
| **Mật khẩu**      | _(xem trong CreateDatabase.sql — phần INSERT INTO TaiKhoan)_ |
| **Vai trò**       | `Admin`                                                      |

> [!CAUTION]
> **Đổi mật khẩu admin ngay sau lần đăng nhập đầu tiên!**

---

## 10. Bảo Trì & Vận Hành

### 10.1. Backup Database

```powershell
# Tạo backup hàng ngày
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = "C:\QuanLyNhanVien\Backup\QuanLyNhanVien_$timestamp.bak"

Invoke-Sqlcmd -ServerInstance "localhost" -TrustServerCertificate `
    -Query "BACKUP DATABASE QuanLyNhanVien TO DISK = N'$backupPath' WITH FORMAT, COMPRESSION"

Write-Host "✅ Backup tạo tại: $backupPath"
```

**Tự động hóa với Task Scheduler:**

```powershell
# Tạo scheduled task backup hàng ngày lúc 2:00 AM
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
    -Argument "-File C:\QuanLyNhanVien\SQL\Backup-Database.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At "02:00AM"
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -RunLevel Highest

Register-ScheduledTask -TaskName "QuanLyNhanVien-DailyBackup" `
    -Action $action -Trigger $trigger -Principal $principal `
    -Description "Backup CSDL Quản Lý Nhân Viên hàng ngày"
```

### 10.2. Dọn Dẹp Log

```powershell
# Chạy stored procedure dọn dẹp ErrorLog (giữ 90 ngày gần nhất)
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_DonDepNhatKy @SoNgayGiu = 90"

# Dọn dẹp file log cũ (giữ 30 file gần nhất)
Get-ChildItem "C:\QuanLyNhanVien\App\Logs\*.log" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -Skip 30 |
    Remove-Item -Force
```

### 10.3. Cập Nhật Phiên Bản Mới

```powershell
# 1. Đóng ứng dụng trên tất cả máy trạm
# 2. Backup toàn bộ trước khi cập nhật
$timestamp = Get-Date -Format "yyyyMMdd"
Copy-Item "C:\QuanLyNhanVien\App" "C:\QuanLyNhanVien\Backup\App_$timestamp" -Recurse

# 3. Copy file mới (giữ nguyên config!)
Copy-Item ".\bin\Release\QuanLyNhanVien.exe" "C:\QuanLyNhanVien\App\" -Force
Copy-Item ".\bin\Release\QuanLyNhanVien.pdb" "C:\QuanLyNhanVien\App\" -Force
# ⚠️ KHÔNG copy QuanLyNhanVien.exe.config — để giữ connection string hiện tại

# 4. Chạy migration SQL mới (nếu có)
# Invoke-Sqlcmd -ServerInstance ... -InputFile "new_migration.sql"

# 5. Khởi động lại ứng dụng
Start-Process "C:\QuanLyNhanVien\App\QuanLyNhanVien.exe"
```

### 10.4. Giám Sát Lỗi

```powershell
# Xem 20 lỗi gần nhất từ ErrorLog
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_DocNhatKy @SoLuong = 20, @MucDo = N'Error'"

# Xem lỗi Critical trong 24h qua
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_DocNhatKy @SoLuong = 50, @MucDo = N'Critical',
            @TuNgay = '$(Get-Date (Get-Date).AddDays(-1) -Format 'yyyy-MM-dd')'"
```

---

## 11. Xử Lý Sự Cố

### 11.1. Ứng dụng không kết nối được đến SQL Server

**Triệu chứng:** Connection Wizard hiện ra mỗi lần mở app, hoặc lỗi "Lỗi kết nối CSDL".

**Kiểm tra theo thứ tự:**

|  #  | Kiểm tra            | Lệnh                                 | Sửa                         |
| :-: | :------------------ | :----------------------------------- | :-------------------------- |
|  1  | SQL Server có chạy? | `Get-Service MSSQLSERVER`            | `Start-Service MSSQLSERVER` |
|  2  | Port 1433 mở?       | `Test-NetConnection <IP> -Port 1433` | Mở firewall (xem mục 4.2)   |
|  3  | TCP/IP bật?         | SQL Server Config Manager            | Bật TCP/IP → Restart        |
|  4  | SA account bật?     | SSMS → Security → Logins → sa        | `ALTER LOGIN sa ENABLE`     |
|  5  | DB tồn tại?         | SSMS → Databases                     | Chạy `Init-Database.ps1`    |

### 11.2. Lỗi "Đăng nhập thất bại" (Error 18456)

```powershell
# Kiểm tra authentication mode
Invoke-Sqlcmd -ServerInstance "localhost" -TrustServerCertificate `
    -Query "SELECT SERVERPROPERTY('IsIntegratedSecurityOnly') AS 'WindowsAuthOnly'"
# Nếu = 1 → Chỉ có Windows Auth → Cần bật Mixed Mode

# Bật Mixed Mode (SQL + Windows Auth)
Invoke-Sqlcmd -ServerInstance "localhost" -TrustServerCertificate `
    -Query "EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE',
            N'Software\Microsoft\MSSQLServer\MSSQLServer', N'LoginMode', REG_DWORD, 2"
# Restart SQL Server sau khi thay đổi
Restart-Service MSSQLSERVER -Force
```

### 11.3. Ứng dụng crash không hiện lỗi

**Bước 1:** Kiểm tra file log:

```powershell
Get-ChildItem "C:\QuanLyNhanVien\App\Logs\" |
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

### 11.4. Hiệu suất chậm

```powershell
# Kiểm tra kích thước database
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "EXEC sp_spaceused"

# Kiểm tra missing indexes
Invoke-Sqlcmd -ServerInstance "localhost" -Database "QuanLyNhanVien" `
    -TrustServerCertificate `
    -Query "SELECT TOP 10
                ROUND(avg_total_user_cost * avg_user_impact * (user_seeks + user_scans), 0) AS [Impact],
                statement AS [Table],
                equality_columns, inequality_columns, included_columns
            FROM sys.dm_db_missing_index_details AS mid
            INNER JOIN sys.dm_db_missing_index_groups AS mig ON mid.index_handle = mig.index_handle
            INNER JOIN sys.dm_db_missing_index_group_stats AS migs ON mig.index_group_handle = migs.group_handle
            WHERE database_id = DB_ID('QuanLyNhanVien')
            ORDER BY [Impact] DESC"
```

---

## 12. Checklist Bàn Giao

### Cho Đội IT Triển Khai

- [ ] **Máy chủ:** Windows Server đã cài đặt và cập nhật
- [ ] **.NET Framework:** Phiên bản 4.7.2+ đã xác nhận
- [ ] **SQL Server:** Đã cài đặt, TCP/IP bật, Mixed Auth mode
- [ ] **Firewall:** Port 1433 TCP đã mở
- [ ] **Database:** Đã chạy `Init-Database.ps1` thành công
- [ ] **Xác nhận bảng:** 9 bảng đã tạo (kiểm tra bằng SSMS)
- [ ] **Ứng dụng:** Đã copy vào `C:\QuanLyNhanVien\App\`
- [ ] **Connection string:** Đã cập nhật trong `.exe.config`
- [ ] **Đăng nhập thử:** Admin login thành công
- [ ] **Log hoạt động:** Kiểm tra file log được tạo trong `Logs/`
- [ ] **Backup:** Scheduled task đã tạo

### Cho Đội Phát Triển

- [ ] **Source code:** Đã push lên repository
- [ ] **SQL scripts:** 3 file migration đều idempotent (chạy lại an toàn)
- [ ] **Build:** Release build thành công (0 errors, 0 warnings)
- [ ] **Installer:** MSI hoặc EXE đã tạo và test
- [ ] **Tài liệu:** Hướng dẫn này + `KE_HOACH_TRIEN_KHAI.md` đã cập nhật
- [ ] **Password mặc định:** Đã thay thế tất cả `YourPassword123!`

### Sản Phẩm Bàn Giao

|  #  | Hạng mục        | File/Thư mục                                               |
| :-: | :-------------- | :--------------------------------------------------------- |
|  1  | Source Code     | Toàn bộ thư mục `QuanLyNhanVien/`                          |
|  2  | SQL Scripts     | `SQL/CreateDatabase.sql`, `SQL/002_*.sql`, `SQL/003_*.sql` |
|  3  | PowerShell Init | `Deploy/Init-Database.ps1`                                 |
|  4  | Installer       | `Installer/Output/QuanLyNhanVien-Setup-v1.0.0.exe`         |
|  5  | Tài liệu        | `KE_HOACH_TRIEN_KHAI.md` + `HUONG_DAN_TRIEN_KHAI.md`       |
|  6  | Báo cáo Word    | _(Theo template Chương 5 trong Kế Hoạch Triển Khai)_       |

---

> **Tài liệu này được cập nhật lần cuối: 2026-02-14**
>
> Phiên bản ứng dụng: **1.0.0**
>
> Liên hệ hỗ trợ: _(điền thông tin liên hệ của đội phát triển)_
