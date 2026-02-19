<#
.SYNOPSIS
    Khởi tạo cơ sở dữ liệu QuanLyNhanVien trên SQL Server.

.DESCRIPTION
    Script này tự động thực thi tất cả SQL migration scripts theo thứ tự đúng:
      1. CreateDatabase.sql   — Tạo database + 4 bảng cốt lõi + dữ liệu mẫu
      2. 002_ExpandSchema.sql — Mở rộng schema: ca làm, chấm công, thưởng/phạt, stored procedures
      3. 003_ErrorLog.sql     — Bảng ErrorLog + stored procedures dọn dẹp

    Tất cả script đều idempotent (chạy lại an toàn, không bị lỗi duplicate).

.PARAMETER ServerInstance
    Tên SQL Server instance. Mặc định: "localhost"
    Ví dụ: ".\SQLEXPRESS", "192.168.1.100", "SERVER01\SQLEXPRESS"

.PARAMETER SqlUsername
    Tài khoản SQL Server. Mặc định: "sa"

.PARAMETER SqlPassword
    Mật khẩu SQL Server. BẮT BUỘC.

.PARAMETER ScriptDirectory
    Thư mục chứa các file SQL. Mặc định: thư mục hiện tại.

.PARAMETER SkipValidation
    Bỏ qua bước kiểm tra kết quả sau khi chạy migration.

.EXAMPLE
    .\Init-Database.ps1 -SqlPassword "MatKhau_Manh!@#456"

.EXAMPLE
    .\Init-Database.ps1 -ServerInstance ".\SQLEXPRESS" -SqlPassword "MyP@ss123!" -ScriptDirectory "C:\SQL"

.EXAMPLE
    # Silent mode cho automation
    .\Init-Database.ps1 -SqlPassword "MyP@ss" -Confirm:$false

.NOTES
    Yêu cầu: SQL Server module (SqlServer hoặc SQLPS)
    Phiên bản: 1.0.0
    Ngày tạo: 2026-02-14
#>

[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter()]
    [string]$ServerInstance = "localhost",

    [Parameter()]
    [string]$SqlUsername = "sa",

    [Parameter(Mandatory = $true)]
    [string]$SqlPassword,

    [Parameter()]
    [string]$ScriptDirectory = $PSScriptRoot,

    [switch]$SkipValidation
)

# ============================================================
# CONFIGURATION
# ============================================================
$ErrorActionPreference = "Stop"
$DatabaseName = "QuanLyNhanVien"

# Thứ tự migration scripts (PHẢI chạy theo đúng thứ tự này)
$migrationScripts = @(
    @{ Name = "CreateDatabase.sql";    Description = "Tạo database + bảng cốt lõi (TaiKhoan, BoPhan, NhanVien, BangLuong)" },
    @{ Name = "002_ExpandSchema.sql";  Description = "Mở rộng schema (CaLamViec, LichLamViec, ChamCong, ThuongPhat, Stored Procedures)" },
    @{ Name = "003_ErrorLog.sql";      Description = "Bảng ErrorLog + SP dọn dẹp nhật ký" }
)

# Danh sách bảng cốt lõi cần kiểm tra
$coreTables = @(
    "TaiKhoan", "BoPhan", "NhanVien", "BangLuong",
    "CaLamViec", "LichLamViec", "ChamCong", "ThuongPhat",
    "ErrorLog"
)

# ============================================================
# HELPER FUNCTIONS
# ============================================================

function Write-Banner {
    param([string]$Text)
    $line = "=" * 60
    Write-Host ""
    Write-Host $line -ForegroundColor Cyan
    Write-Host "  $Text" -ForegroundColor Cyan
    Write-Host $line -ForegroundColor Cyan
    Write-Host ""
}

function Write-Step {
    param([int]$Number, [string]$Text)
    Write-Host "  [$Number] " -ForegroundColor Yellow -NoNewline
    Write-Host $Text
}

function Write-Success {
    param([string]$Text)
    Write-Host "  ✅ " -ForegroundColor Green -NoNewline
    Write-Host $Text -ForegroundColor Green
}

function Write-Failure {
    param([string]$Text)
    Write-Host "  ❌ " -ForegroundColor Red -NoNewline
    Write-Host $Text -ForegroundColor Red
}

function Write-Info {
    param([string]$Text)
    Write-Host "  ℹ️  " -ForegroundColor DarkCyan -NoNewline
    Write-Host $Text -ForegroundColor Gray
}

function Test-SqlModule {
    <#
    .SYNOPSIS
        Kiểm tra và import SQL Server module.
    #>
    
    # Ưu tiên module SqlServer (mới hơn)
    if (Get-Module -ListAvailable -Name SqlServer) {
        Import-Module SqlServer -DisableNameChecking
        Write-Info "Sử dụng module: SqlServer"
        return $true
    }
    
    # Fallback sang SQLPS (cũ hơn, đi kèm SSMS)
    if (Get-Module -ListAvailable -Name SQLPS) {
        Push-Location  # SQLPS changes the directory
        Import-Module SQLPS -DisableNameChecking
        Pop-Location
        Write-Info "Sử dụng module: SQLPS"
        return $true
    }
    
    return $false
}

function Test-SqlConnection {
    <#
    .SYNOPSIS
        Kiểm tra kết nối TCP đến SQL Server.
    #>
    param(
        [string]$Server,
        [int]$Port = 1433
    )
    
    # Trích xuất server name và port
    $parts = $Server -split ","
    $hostname = ($parts[0] -split "\\")[0]  # Lấy hostname, bỏ instance name
    if ($parts.Count -gt 1) { $Port = [int]$parts[1] }
    
    try {
        $tcp = New-Object System.Net.Sockets.TcpClient
        $asyncResult = $tcp.BeginConnect($hostname, $Port, $null, $null)
        $success = $asyncResult.AsyncWaitHandle.WaitOne(5000)  # 5 second timeout
        
        if ($success) {
            $tcp.EndConnect($asyncResult)
            $tcp.Close()
            return $true
        }
        else {
            $tcp.Close()
            return $false
        }
    }
    catch {
        return $false
    }
}

function Invoke-SqlFile {
    <#
    .SYNOPSIS
        Thực thi một file SQL với xử lý GO batch separator.
    #>
    param(
        [string]$FilePath,
        [string]$Server,
        [string]$Username,
        [string]$Password
    )
    
    # Đọc nội dung file
    $content = Get-Content $FilePath -Raw -Encoding UTF8
    
    # Tách theo GO statements (mỗi batch riêng)
    $batches = $content -split "(?m)^\s*GO\s*$" | Where-Object { $_.Trim() -ne "" }
    
    $batchCount = 0
    $errorCount = 0
    
    foreach ($batch in $batches) {
        $trimmed = $batch.Trim()
        if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }
        
        try {
            Invoke-Sqlcmd -ServerInstance $Server `
                -Username $Username -Password $Password `
                -Query $trimmed `
                -TrustServerCertificate `
                -QueryTimeout 120 `
                -ErrorAction Stop
            
            $batchCount++
        }
        catch {
            $errorCount++
            $errorMsg = $_.Exception.Message
            
            # Bỏ qua lỗi "already exists" (idempotent scripts)
            if ($errorMsg -match "already exists|already an object|duplicate") {
                Write-Info "Bỏ qua (đã tồn tại): $($errorMsg.Substring(0, [Math]::Min($errorMsg.Length, 80)))..."
                continue
            }
            
            Write-Failure "Lỗi batch #$($batchCount + 1): $errorMsg"
        }
    }
    
    return @{ Batches = $batchCount; Errors = $errorCount }
}

# ============================================================
# MAIN EXECUTION
# ============================================================

$startTime = Get-Date

Write-Banner "KHỞI TẠO CƠ SỞ DỮ LIỆU — QUẢN LÝ NHÂN VIÊN QUÁN ĂN"

Write-Host "  Thông tin kết nối:" -ForegroundColor White
Write-Host "    Server:    $ServerInstance" -ForegroundColor Gray
Write-Host "    Username:  $SqlUsername" -ForegroundColor Gray
Write-Host "    Database:  $DatabaseName" -ForegroundColor Gray
Write-Host "    Scripts:   $ScriptDirectory" -ForegroundColor Gray
Write-Host ""

# ─────────────────────────────────────────────────────
# STEP 0: Kiểm tra điều kiện tiên quyết
# ─────────────────────────────────────────────────────
Write-Banner "Bước 0: Kiểm Tra Điều Kiện Tiên Quyết"

# Check SQL module
Write-Step 1 "Kiểm tra SQL Server PowerShell module..."
if (-not (Test-SqlModule)) {
    Write-Failure "Không tìm thấy module SqlServer hoặc SQLPS."
    Write-Host ""
    Write-Host "  Cài đặt bằng lệnh:" -ForegroundColor Yellow
    Write-Host "    Install-Module -Name SqlServer -AllowClobber -Scope CurrentUser" -ForegroundColor White
    Write-Host ""
    exit 1
}
Write-Success "SQL module sẵn sàng."

# Check SQL scripts exist
Write-Step 2 "Kiểm tra file SQL migration..."
$missingFiles = @()
foreach ($script in $migrationScripts) {
    $fullPath = Join-Path $ScriptDirectory $script.Name
    if (-not (Test-Path $fullPath)) {
        $missingFiles += $script.Name
        Write-Failure "Không tìm thấy: $($script.Name)"
    }
    else {
        $fileSize = (Get-Item $fullPath).Length
        Write-Info "$($script.Name) ($([Math]::Round($fileSize/1024, 1)) KB)"
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host ""
    Write-Failure "Thiếu $($missingFiles.Count) file SQL. Đảm bảo tất cả file nằm trong: $ScriptDirectory"
    exit 1
}
Write-Success "Tất cả $($migrationScripts.Count) file SQL đã sẵn sàng."

# Check TCP connectivity
Write-Step 3 "Kiểm tra kết nối TCP đến SQL Server..."
if (-not (Test-SqlConnection -Server $ServerInstance)) {
    Write-Failure "Không thể kết nối đến $ServerInstance trên port 1433."
    Write-Host ""
    Write-Host "  Kiểm tra:" -ForegroundColor Yellow
    Write-Host "    1. SQL Server service đang chạy:  Get-Service MSSQLSERVER" -ForegroundColor Gray
    Write-Host "    2. TCP/IP đã bật:                 SQL Server Configuration Manager" -ForegroundColor Gray
    Write-Host "    3. Firewall cho phép port 1433:   Test-NetConnection $ServerInstance -Port 1433" -ForegroundColor Gray
    Write-Host ""
    exit 1
}
Write-Success "Kết nối TCP thành công."

# Check SQL authentication
Write-Step 4 "Kiểm tra xác thực SQL Server..."
try {
    Invoke-Sqlcmd -ServerInstance $ServerInstance `
        -Username $SqlUsername -Password $SqlPassword `
        -Query "SELECT @@VERSION AS [Version]" `
        -TrustServerCertificate `
        -ErrorAction Stop | Out-Null
    
    $version = Invoke-Sqlcmd -ServerInstance $ServerInstance `
        -Username $SqlUsername -Password $SqlPassword `
        -Query "SELECT SERVERPROPERTY('ProductVersion') AS [Ver], SERVERPROPERTY('Edition') AS [Ed]" `
        -TrustServerCertificate
    
    Write-Success "Xác thực thành công."
    Write-Info "SQL Server $($version.Ver) — $($version.Ed)"
}
catch {
    Write-Failure "Xác thực thất bại: $($_.Exception.Message)"
    Write-Host ""
    Write-Host "  Kiểm tra:" -ForegroundColor Yellow
    Write-Host "    1. Mixed authentication mode đã bật" -ForegroundColor Gray
    Write-Host "    2. Tài khoản SA đã enable:  ALTER LOGIN sa ENABLE" -ForegroundColor Gray
    Write-Host "    3. Mật khẩu đúng" -ForegroundColor Gray
    Write-Host ""
    exit 1
}

# ─────────────────────────────────────────────────────
# STEP 1-3: Chạy Migration Scripts
# ─────────────────────────────────────────────────────
Write-Banner "Chạy Migration Scripts"

$totalBatches = 0
$totalErrors = 0
$scriptIndex = 0

foreach ($script in $migrationScripts) {
    $scriptIndex++
    $fullPath = Join-Path $ScriptDirectory $script.Name
    
    Write-Step $scriptIndex "$($script.Name)"
    Write-Info $script.Description
    
    $stepStart = Get-Date
    
    if ($PSCmdlet.ShouldProcess($script.Name, "Execute SQL migration")) {
        $result = Invoke-SqlFile -FilePath $fullPath `
            -Server $ServerInstance `
            -Username $SqlUsername `
            -Password $SqlPassword
        
        $duration = (Get-Date) - $stepStart
        $totalBatches += $result.Batches
        $totalErrors += $result.Errors
        
        if ($result.Errors -eq 0) {
            Write-Success "$($result.Batches) batches thực thi thành công ($([Math]::Round($duration.TotalSeconds, 1))s)"
        }
        else {
            Write-Failure "$($result.Errors) lỗi trong $($result.Batches) batches ($([Math]::Round($duration.TotalSeconds, 1))s)"
        }
    }
    
    Write-Host ""
}

# ─────────────────────────────────────────────────────
# STEP 4: Kiểm Tra Kết Quả
# ─────────────────────────────────────────────────────
if (-not $SkipValidation) {
    Write-Banner "Kiểm Tra Kết Quả"
    
    # Check database exists
    Write-Step 1 "Kiểm tra database..."
    try {
        $dbCheck = Invoke-Sqlcmd -ServerInstance $ServerInstance `
            -Username $SqlUsername -Password $SqlPassword `
            -Query "SELECT DB_ID('$DatabaseName') AS [DBID]" `
            -TrustServerCertificate
        
        if ($null -ne $dbCheck.DBID) {
            Write-Success "Database '$DatabaseName' tồn tại."
        }
        else {
            Write-Failure "Database '$DatabaseName' KHÔNG tồn tại!"
        }
    }
    catch {
        Write-Failure "Không thể kiểm tra database: $($_.Exception.Message)"
    }
    
    # Check tables
    Write-Step 2 "Kiểm tra bảng..."
    try {
        $tables = Invoke-Sqlcmd -ServerInstance $ServerInstance `
            -Username $SqlUsername -Password $SqlPassword `
            -Database $DatabaseName `
            -Query "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME" `
            -TrustServerCertificate
        
        $existingTables = $tables | ForEach-Object { $_.TABLE_NAME }
        $missingTables = @()
        
        foreach ($table in $coreTables) {
            if ($existingTables -contains $table) {
                Write-Success "Bảng: $table"
            }
            else {
                Write-Failure "Thiếu bảng: $table"
                $missingTables += $table
            }
        }
        
        # List any extra tables not in our core list
        $extraTables = $existingTables | Where-Object { $coreTables -notcontains $_ }
        foreach ($extra in $extraTables) {
            Write-Info "Bảng bổ sung: $extra"
        }
        
        Write-Host ""
        if ($missingTables.Count -eq 0) {
            Write-Success "Tất cả $($coreTables.Count) bảng cốt lõi đã tạo thành công!"
        }
        else {
            Write-Failure "Thiếu $($missingTables.Count) bảng: $($missingTables -join ', ')"
        }
    }
    catch {
        Write-Failure "Không thể kiểm tra bảng: $($_.Exception.Message)"
    }
    
    # Check stored procedures
    Write-Step 3 "Kiểm tra stored procedures..."
    try {
        $procs = Invoke-Sqlcmd -ServerInstance $ServerInstance `
            -Username $SqlUsername -Password $SqlPassword `
            -Database $DatabaseName `
            -Query "SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' ORDER BY ROUTINE_NAME" `
            -TrustServerCertificate
        
        if ($procs) {
            foreach ($proc in $procs) {
                Write-Success "SP: $($proc.ROUTINE_NAME)"
            }
        }
        else {
            Write-Info "Không có stored procedures."
        }
    }
    catch {
        Write-Failure "Không thể kiểm tra stored procedures: $($_.Exception.Message)"
    }
    
    # Check sample data
    Write-Step 4 "Kiểm tra dữ liệu mẫu..."
    try {
        $counts = Invoke-Sqlcmd -ServerInstance $ServerInstance `
            -Username $SqlUsername -Password $SqlPassword `
            -Database $DatabaseName `
            -Query "SELECT 
                        (SELECT COUNT(*) FROM TaiKhoan) AS TaiKhoan,
                        (SELECT COUNT(*) FROM BoPhan)   AS BoPhan,
                        (SELECT COUNT(*) FROM NhanVien) AS NhanVien" `
            -TrustServerCertificate
        
        Write-Info "TaiKhoan: $($counts.TaiKhoan) records | BoPhan: $($counts.BoPhan) records | NhanVien: $($counts.NhanVien) records"
        
        if ($counts.TaiKhoan -eq 0) {
            Write-Failure "Chưa có tài khoản admin! Kiểm tra lại CreateDatabase.sql → phần INSERT INTO TaiKhoan."
        }
        else {
            Write-Success "Dữ liệu mẫu đã sẵn sàng."
        }
    }
    catch {
        Write-Failure "Không thể kiểm tra dữ liệu: $($_.Exception.Message)"
    }
}

# ─────────────────────────────────────────────────────
# SUMMARY
# ─────────────────────────────────────────────────────
$totalDuration = (Get-Date) - $startTime

Write-Banner "KẾT QUẢ KHỞI TẠO"

Write-Host "  Tổng thời gian:    $([Math]::Round($totalDuration.TotalSeconds, 1)) giây" -ForegroundColor White
Write-Host "  Tổng batches:      $totalBatches" -ForegroundColor White
Write-Host "  Lỗi:               $totalErrors" -ForegroundColor $(if ($totalErrors -gt 0) { 'Red' } else { 'Green' })
Write-Host ""

if ($totalErrors -eq 0) {
    Write-Host "  ╔══════════════════════════════════════════════════╗" -ForegroundColor Green
    Write-Host "  ║                                                  ║" -ForegroundColor Green
    Write-Host "  ║   ✅  CƠ SỞ DỮ LIỆU ĐÃ KHỞI TẠO THÀNH CÔNG!  ║" -ForegroundColor Green
    Write-Host "  ║                                                  ║" -ForegroundColor Green
    Write-Host "  ╚══════════════════════════════════════════════════╝" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Bước tiếp theo:" -ForegroundColor Yellow
    Write-Host "    1. Cập nhật connection string trong QuanLyNhanVien.exe.config" -ForegroundColor Gray
    Write-Host "    2. Chạy ứng dụng và đăng nhập bằng tài khoản admin" -ForegroundColor Gray
    Write-Host "    3. Đổi mật khẩu admin sau lần đăng nhập đầu tiên" -ForegroundColor Gray
}
else {
    Write-Host "  ╔══════════════════════════════════════════════════╗" -ForegroundColor Red
    Write-Host "  ║                                                  ║" -ForegroundColor Red
    Write-Host "  ║   ❌  CÓ LỖI TRONG QUÁ TRÌNH KHỞI TẠO!        ║" -ForegroundColor Red
    Write-Host "  ║                                                  ║" -ForegroundColor Red
    Write-Host "  ╚══════════════════════════════════════════════════╝" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Đề xuất:" -ForegroundColor Yellow
    Write-Host "    1. Xem lại lỗi ở trên" -ForegroundColor Gray
    Write-Host "    2. Kiểm tra SQL Server error log: SSMS → Management → SQL Server Logs" -ForegroundColor Gray
    Write-Host "    3. Chạy lại script (scripts là idempotent, an toàn khi chạy nhiều lần)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "  Connection string cho ứng dụng:" -ForegroundColor Yellow
Write-Host "    Server=$ServerInstance;Database=$DatabaseName;User Id=$SqlUsername;Password=***;TrustServerCertificate=True" -ForegroundColor White
Write-Host ""

# Return exit code
if ($totalErrors -gt 0) { exit 1 }
exit 0
