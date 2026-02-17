# CORS Configuration for .NET Backend

## The Problem
You're getting "Failed to fetch" errors because the backend at `https://localhost:7020` is not configured to accept requests from your Angular app at `http://localhost:4200`.

## Solution: Add CORS to Your .NET Backend

### Step 1: Update `Program.cs`

Add CORS configuration in your .NET backend's `Program.cs` file:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Other services...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS - MUST be before UseAuthorization
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 2: Restart Your .NET Backend

After making these changes:
1. Stop your .NET backend (if running)
2. Rebuild and run it again

```bash
cd ..\DotNetAngularApp
dotnet run
```

### Step 3: Test Again

Once the backend is running with CORS enabled:
1. Refresh your Angular app in the browser
2. Try creating a category again
3. The "Failed to fetch" error should be gone

## Alternative: Development-Only Permissive CORS

For development only, you can use a more permissive policy:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Then use it:
app.UseCors("DevelopmentCors");
```

**⚠️ Warning:** Don't use AllowAnyOrigin in production!

## Verify CORS is Working

Open browser DevTools (F12) → Network tab → Try creating a category:
- You should see a 201 Created response instead of "Failed to fetch"
- Response headers should include `Access-Control-Allow-Origin`
