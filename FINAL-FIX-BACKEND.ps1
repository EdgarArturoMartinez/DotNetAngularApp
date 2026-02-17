# ========================================
# DEFINITIVE FIX FOR "NO ROUTE MATCHES" ERROR
# ========================================
# This script will completely restart the backend after all fixes

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "BACKEND RESTART - FINAL FIX" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Kill any running dotnet processes
Write-Host "[1/5] Killing existing .NET processes..." -ForegroundColor Green
Get-Process | Where-Object { $_.ProcessName -match 'dotnet' } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "✓ Done" -ForegroundColor Green
Write-Host ""

# Step 2: Navigate to backend directory
Write-Host "[2/5] Navigating to backend directory..." -ForegroundColor Green
$backendPath = "c:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi"
Set-Location $backendPath
Write-Host "✓ In directory: $(Get-Location)" -ForegroundColor Green
Write-Host ""

# Step 3: Clean the build
Write-Host "[3/5] Cleaning previous build..." -ForegroundColor Green
dotnet clean -nologo
Start-Sleep -Seconds 1
Write-Host "✓ Done" -ForegroundColor Green
Write-Host ""

# Step 4: Restore dependencies
Write-Host "[4/5] Restoring NuGet packages..." -ForegroundColor Green
dotnet restore -nologo
Start-Sleep -Seconds 1
Write-Host "✓ Done" -ForegroundColor Green
Write-Host ""

# Step 5: Start the backend
Write-Host "[5/5] Starting .NET backend on https://localhost:7020" -ForegroundColor Green
Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Backend is starting..." -ForegroundColor Yellow
Write-Host "IMPORTANT: Keep this window open!" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

dotnet run

