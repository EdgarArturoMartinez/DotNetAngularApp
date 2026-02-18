Write-Host "`n=== Restarting Backend with StockQuantity Fix ===" -ForegroundColor Cyan
Write-Host "Stopping all backend processes..." -ForegroundColor Yellow

# Kill all DotNetCoreWebApi processes
Get-Process | Where-Object { $_.Name -eq 'DotNetCoreWebApi' } | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process | Where-Object { $_.ProcessName -match 'dotnet' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
Set-Location "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
dotnet clean

Write-Host "`nStarting backend server..." -ForegroundColor Yellow
dotnet run
