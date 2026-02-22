using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure;

/// <summary>
/// Database seeder to ensure initial data exists
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the database with initial admin user if no admin exists
    /// </summary>
    public static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDBContext>>();

        try
        {
            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            // Check if any admin user already exists
            var adminExists = await context.Customers.AnyAsync(c => c.Role == UserRole.Admin);

            if (!adminExists)
            {
                logger.LogInformation("No admin user found. Creating default admin user...");

                // Get admin credentials from configuration or use defaults
                var adminEmail = configuration["AdminUser:Email"] ?? "admin@veggyworld.com";
                var adminPassword = configuration["AdminUser:Password"] ?? "Admin@2026!";
                var adminFirstName = configuration["AdminUser:FirstName"] ?? "System";
                var adminLastName = configuration["AdminUser:LastName"] ?? "Administrator";

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                // Create admin user
                var adminUser = new Customer
                {
                    Email = adminEmail,
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    PhoneNumber = "",
                    PasswordHash = passwordHash,
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Customers.AddAsync(adminUser);
                await context.SaveChangesAsync();

                logger.LogInformation("Default admin user created successfully:");
                logger.LogInformation("  Email: {Email}", adminEmail);
                logger.LogInformation("  Password: {Password}", adminPassword);
                logger.LogWarning("IMPORTANT: Please change the admin password after first login!");
            }
            else
            {
                logger.LogInformation("Admin user(s) already exist. Skipping admin user creation.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
