# Script to restart the .NET backend
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Backend Restart Helper" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
$process = Get-Process -Name DotNetCoreWebApi -ErrorAction SilentlyContinue

if ($process) {
    Write-Host "✗ Backend is currently running (PID: $($process.Id))" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "MANUAL STEP REQUIRED:" -ForegroundColor Red
    Write-Host "1. Find the PowerShell window where 'dotnet run' is running" -ForegroundColor White
    Write-Host "2. Press Ctrl+C in that window to stop the server" -ForegroundColor White
    Write-Host "3. Run this script again to start the server" -ForegroundColor White
    Write-Host ""
    Write-Host "OR use Task Manager:" -ForegroundColor White
    Write-Host "1. Open Task Manager (Ctrl+Shift+Esc)" -ForegroundColor White
    Write-Host "2. Find 'DotNetCoreWebApi.exe' under Details tab" -ForegroundColor White
    Write-Host "3. Right-click and select 'End task'" -ForegroundColor White
    Write-Host "4. Run this script again" -ForegroundColor White
    exit
}

Write-Host "✓ No backend process found. Starting server..." -ForegroundColor Green
Write-Host ""

# Start the backend
dotnet run
