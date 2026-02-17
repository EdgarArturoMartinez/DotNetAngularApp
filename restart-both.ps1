Write-Host "`n=== RESTARTING BACKEND AND FRONTEND ===" -ForegroundColor Cyan
Write-Host ""

# Stop backend
Write-Host "Stopping backend on port 7020..." -ForegroundColor Yellow
$backendProc = Get-NetTCPConnection -LocalPort 7020 -ErrorAction SilentlyContinue | Select-Object -First 1
if ($backendProc) {
    Stop-Process -Id $backendProc.OwningProcess -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    Write-Host "✓ Backend stopped" -ForegroundColor Green
} else {
    Write-Host "✓ No backend process found" -ForegroundColor Green
}

# Stop Angular
Write-Host "Stopping Angular on port 4200..." -ForegroundColor Yellow
Get-Process node -ErrorAction SilentlyContinue | ForEach-Object {
    $conns = Get-NetTCPConnection -OwningProcess $_.Id -ErrorAction SilentlyContinue | Where-Object {$_.LocalPort -eq 4200}
    if ($conns) {
        Stop-Process -Id $_.Id -Force
    }
}
Start-Sleep -Seconds 2
Write-Host "✓ Angular stopped" -ForegroundColor Green

Write-Host ""
Write-Host "=== SERVERS STOPPED ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOW DO THIS:" -ForegroundColor Yellow
Write-Host "1. Open a terminal and run:" -ForegroundColor White
Write-Host "   cd C:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Open another terminal and run:" -ForegroundColor White
Write-Host "   cd C:\Arthur\Development\2026\DotNetAngularApp\angular-app" -ForegroundColor Cyan
Write-Host "   ng serve -o" -ForegroundColor Cyan
Write-Host ""
