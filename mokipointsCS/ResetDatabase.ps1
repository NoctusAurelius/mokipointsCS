# Reset Database Script for Mokipoints
# This script deletes the database files and lets the DatabaseInitializer recreate them

Write-Host "=== Mokipoints Database Reset Script ===" -ForegroundColor Cyan
Write-Host ""

# Stop IIS Express if running
Write-Host "Checking for running IIS Express processes..." -ForegroundColor Yellow
$iisProcesses = Get-Process | Where-Object {$_.ProcessName -like "*iisexpress*"}
if ($iisProcesses) {
    Write-Host "Stopping IIS Express..." -ForegroundColor Yellow
    $iisProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
    Write-Host "IIS Express stopped." -ForegroundColor Green
} else {
    Write-Host "No IIS Express processes found. Continuing..." -ForegroundColor Green
}

# Navigate to App_Data
$appDataPath = Join-Path $PSScriptRoot "App_Data"
if (-not (Test-Path $appDataPath)) {
    Write-Host "ERROR: App_Data folder not found at: $appDataPath" -ForegroundColor Red
    exit 1
}

# Delete database files
$mdfFile = Join-Path $appDataPath "Mokipoints.mdf"
$ldfFile = Join-Path $appDataPath "Mokipoints_log.ldf"

if (Test-Path $mdfFile) {
    try {
        Remove-Item $mdfFile -Force -ErrorAction Stop
        Write-Host "✓ Deleted: Mokipoints.mdf" -ForegroundColor Green
    } catch {
        Write-Host "ERROR: Could not delete Mokipoints.mdf - File may be locked" -ForegroundColor Red
        Write-Host "Please close Visual Studio and try again." -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "ℹ Mokipoints.mdf not found (may already be deleted)" -ForegroundColor Yellow
}

if (Test-Path $ldfFile) {
    try {
        Remove-Item $ldfFile -Force -ErrorAction Stop
        Write-Host "✓ Deleted: Mokipoints_log.ldf" -ForegroundColor Green
    } catch {
        Write-Host "WARNING: Could not delete Mokipoints_log.ldf" -ForegroundColor Yellow
    }
} else {
    Write-Host "ℹ Mokipoints_log.ldf not found (may already be deleted)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Database Reset Complete ===" -ForegroundColor Green
Write-Host "When you restart the application, the database will be automatically recreated with empty tables." -ForegroundColor Cyan
Write-Host ""

