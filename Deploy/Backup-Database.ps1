<#
.SYNOPSIS
    Backup cơ sở dữ liệu QuanLyNhanVien và dọn dẹp backup cũ.

.DESCRIPTION
    Script này tạo backup nén (.bak) và xóa các backup cũ hơn số ngày cấu hình.
    Thiết kế để chạy tự động qua Windows Task Scheduler.

.PARAMETER ServerInstance
    SQL Server instance. Mặc định: "localhost"

.PARAMETER SqlUsername
    Tài khoản SQL. Mặc định: "sa"

.PARAMETER SqlPassword
    Mật khẩu SQL. BẮT BUỘC.

.PARAMETER BackupDirectory
    Thư mục lưu backup. Mặc định: "C:\QuanLyNhanVien\Backup"

.PARAMETER RetentionDays
    Số ngày giữ backup. Mặc định: 30

.EXAMPLE
    .\Backup-Database.ps1 -SqlPassword "MyP@ss123!"
#>

[CmdletBinding()]
param(
    [string]$ServerInstance = "localhost",
    [string]$SqlUsername = "sa",
    [Parameter(Mandatory = $true)]
    [string]$SqlPassword,
    [string]$BackupDirectory = "C:\QuanLyNhanVien\Backup",
    [int]$RetentionDays = 30
)

$ErrorActionPreference = "Stop"
$DatabaseName = "QuanLyNhanVien"

# Import SQL module
if (Get-Module -ListAvailable -Name SqlServer) {
    Import-Module SqlServer -DisableNameChecking
} elseif (Get-Module -ListAvailable -Name SQLPS) {
    Push-Location
    Import-Module SQLPS -DisableNameChecking
    Pop-Location
} else {
    Write-Error "SQL Server PowerShell module not found."
    exit 1
}

# Ensure backup directory exists
if (-not (Test-Path $BackupDirectory)) {
    New-Item -ItemType Directory -Path $BackupDirectory -Force | Out-Null
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = Join-Path $BackupDirectory "${DatabaseName}_${timestamp}.bak"
$logFile = Join-Path $BackupDirectory "backup_log.txt"

try {
    # Create backup with compression
    $query = @"
BACKUP DATABASE [$DatabaseName] 
TO DISK = N'$backupFile' 
WITH FORMAT, COMPRESSION, STATS = 25,
     NAME = N'$DatabaseName - Full Backup $timestamp',
     DESCRIPTION = N'Automatic daily backup'
"@

    Write-Host "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] Bắt đầu backup..." -ForegroundColor Cyan
    
    Invoke-Sqlcmd -ServerInstance $ServerInstance `
        -Username $SqlUsername -Password $SqlPassword `
        -Query $query `
        -TrustServerCertificate `
        -QueryTimeout 600

    $fileSize = [Math]::Round((Get-Item $backupFile).Length / 1MB, 2)
    $message = "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] ✅ Backup thành công: $backupFile ($fileSize MB)"
    Write-Host $message -ForegroundColor Green
    Add-Content $logFile $message

    # Cleanup old backups
    $cutoffDate = (Get-Date).AddDays(-$RetentionDays)
    $oldBackups = Get-ChildItem "$BackupDirectory\*.bak" |
        Where-Object { $_.LastWriteTime -lt $cutoffDate }

    if ($oldBackups) {
        foreach ($old in $oldBackups) {
            Remove-Item $old.FullName -Force
            $delMsg = "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] 🗑️  Xóa backup cũ: $($old.Name)"
            Write-Host $delMsg -ForegroundColor DarkYellow
            Add-Content $logFile $delMsg
        }
    }

    # Cleanup ErrorLog table (keep 90 days)
    try {
        Invoke-Sqlcmd -ServerInstance $ServerInstance `
            -Username $SqlUsername -Password $SqlPassword `
            -Database $DatabaseName `
            -Query "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_DonDepNhatKy') EXEC sp_DonDepNhatKy @SoNgayGiu = 90" `
            -TrustServerCertificate
        
        Write-Host "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] 🧹 Dọn dẹp ErrorLog (giữ 90 ngày)." -ForegroundColor Gray
    }
    catch {
        Write-Host "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] ⚠️ Không thể dọn ErrorLog: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
catch {
    $errMsg = "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] ❌ Backup THẤT BẠI: $($_.Exception.Message)"
    Write-Host $errMsg -ForegroundColor Red
    Add-Content $logFile $errMsg
    exit 1
}
