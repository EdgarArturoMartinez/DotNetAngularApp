# How to Restart Your Backend

## QUICK FIX - Backend needs restart:

The edit category function requires a backend restart to work.

### Step 1: Stop the Current Backend
Press `Ctrl+C` in the terminal window running `dotnet run`

### Step 2: Start the Backend
In that same terminal, run:
```powershell
dotnet run
```

Or open a new PowerShell terminal and run:
```powershell
cd C:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi
dotnet run
```

### Step 3: Test It's Working
```powershell
curl.exe -k https://localhost:7020/api/vegcategories/1
```

You should see category data without the `vegProducts` array.

---

## ORIGINAL INSTRUCTIONS (for reference)

## THE PROBLEM
The backend had a circular reference bug that was causing the products API to fail. This has been fixed using DTOs (Data Transfer Objects).

## TO GET EVERYTHING WORKING:

### Step 1: Stop the Current Backend
The backend is currently running but with old code. You need to stop it:

**Option A - Find the terminal window:**
1. Look for a PowerShell or Terminal window that says "dotnet run" or shows the backend running
2. Click on that window
3. Press `Ctrl+C` to stop the server

**Option B - Use Task Manager:**
1. Press `Ctrl+Shift+Esc` to open Task Manager
2. Click on the "Details" tab
3. Find `DotNetCoreWebApi.exe`
4. Right-click it and select "End task"

### Step 2: Start the Backend with the Fix
Open a new PowerShell terminal in VS Code and run:
```powershell
cd C:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi
.\restart-backend.ps1
```

Or manually:
```powershell
cd C:\Arthur\Development\2026\DotNetAngularApp\DotNetCoreWebApi\DotNetCoreWebApi
dotnet run
```

### Step 3: Verify It's Working
Once the backend starts (you'll see "Now listening on: https://localhost:7020"), test it:
```powershell
curl.exe -k https://localhost:7020/api/vegproducts
```

You should see JSON data with products (not an error about circular references).

### Step 4: Test in Browser
1. Make sure Angular is running on http://localhost:4200
2. Go to http://localhost:4200/products - You should see your products
3. Go to http://localhost:4200/categories - You should see your categories
4. Try creating a new category and assigning it to a product

## WHAT WAS FIXED:
- ✅ Created DTOs to prevent circular references (VegProductDto, VegCategoryDto)
- ✅ Updated VegProductsController to return DTOs instead of entities
- ✅ Updated VegCategoriesController to return DTOs with product counts
- ✅ Added GetById endpoint for categories (needed for edit function)
- ✅ Fixed environment.development.ts to use HTTPS instead of HTTP
- ✅ Backend rebuilt successfully with all fixes

## CURRENT STATUS:
- ✅ Angular: Running on port 4200
- ✅ Backend Code: Fixed and built successfully
- ⏳ Backend Server: Needs restart to load the fixes
