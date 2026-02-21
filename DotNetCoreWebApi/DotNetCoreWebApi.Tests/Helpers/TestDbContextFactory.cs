using DotNetCoreWebApi.Application.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Tests.Helpers;

/// <summary>
/// Factory for creating in-memory database contexts for testing
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a new DbContext with an in-memory database
    /// Each call creates a new isolated database instance
    /// </summary>
    public static ApplicationDBContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDBContext(options);
        return context;
    }

    /// <summary>
    /// Creates a DbContext and seeds it with initial test data
    /// </summary>
    public static ApplicationDBContext CreateSeededContext()
    {
        var context = CreateInMemoryContext();
        SeedTestData(context);
        return context;
    }

    /// <summary>
    /// Seeds the database with common test data
    /// </summary>
    private static void SeedTestData(ApplicationDBContext context)
    {
        // Add test categories
        var categories = MockDataGenerator.GenerateCategories(5);
        context.VegCategories.AddRange(categories);
        context.SaveChanges();

        // Add test products
        var products = MockDataGenerator.GenerateProducts(10, categories);
        context.VegProducts.AddRange(products);
        context.SaveChanges();
    }
}
