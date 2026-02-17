Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CLEAN RESTART - Both Servers" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Stop Backend
Write-Host "[1/4] Stopping Backend..." -ForegroundColor Yellow
Stop-Process -Name "DotNetCoreWebApi" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "      Backend stopped" -ForegroundColor Green
Write-Host ""

# Stop Angular
Write-Host "[2/4] Stopping Angular..." -ForegroundColor Yellow
Get-Process node -ErrorAction SilentlyContinue | ForEach-Object {
    $conns = Get-NetTCPConnection -OwningProcess $_.Id -ErrorAction SilentlyContinue | Where-Object {$_.LocalPort -eq 4200}
    if ($conns) {
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
}
Start-Sleep -Seconds 2
Write-Host "      Angular stopped" -ForegroundColor Green
Write-Host ""

# Start Backend
Write-Host "[3/4] Starting Backend..." -ForegroundColor Yellow
Set-Location "C:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run"
Start-Sleep -Seconds 5
Write-Host "      Backend starting (check new window)..." -ForegroundColor Green
Write-Host ""

# Start Angular  
Write-Host "[4/4] Starting Angular..." -ForegroundColor Yellow
Set-Location "C:\Arthur\Development\2026\DotNetAngularApp\angular-app"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "ng serve --port 4200 -o"
Start-Sleep -Seconds 3
Write-Host "      Angular starting (check new window)..." -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Both servers started!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backend: https://localhost:7020" -ForegroundColor White
Write-Host "Angular: http://localhost:4200" -ForegroundColor White
Write-Host ""
Write-Host "Two new PowerShell windows opened." -ForegroundColor Yellow
Write-Host "Wait for both to finish starting, then test your app!" -ForegroundColor Yellow
Write-Host ""
