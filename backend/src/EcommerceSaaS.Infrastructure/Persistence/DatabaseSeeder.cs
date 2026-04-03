using EcommerceSaaS.Domain.Entities;
using EcommerceSaaS.Domain.Enums;
using EcommerceSaaS.Infrastructure.Persistence;
using BCrypt.Net;

namespace EcommerceSaaS.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            // Check if data already exists
            if (context.Tenants.Any() || context.Users.Any())
            {
                return;
            }

            // Seed Tenants
            var superAdminTenant = new Tenant("EcommerceSaaS", "ecommerce-saas", SubscriptionPlan.Enterprise)
            {
                TenantId = Guid.Empty, // SuperAdmin tenant has empty ID
                Domain = "localhost"
            };

            var vendorTenant = new Tenant("Tech Store", "tech-store", SubscriptionPlan.Professional)
            {
                Domain = "shop.localhost"
            };

            var customerTenant = new Tenant("Fashion Hub", "fashion-hub", SubscriptionPlan.Starter)
            {
                Domain = "fashion.localhost"
            };

            await context.Tenants.AddAsync(superAdminTenant);
            await context.Tenants.AddAsync(vendorTenant);
            await context.Tenants.AddAsync(customerTenant);

            // Seed Users
            var superAdminUser = new User(
                "admin@ecommercesaas.com",
                "Admin",
                "User",
                UserRole.SuperAdmin,
                Guid.Empty
            )
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsEmailConfirmed = true,
                IsActive = true
            };

            var vendorUser = new User(
                "vendor@techstore.com",
                "vendor",
                "techstore",
                UserRole.Vendor,
                vendorTenant.TenantId
            )
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor123!"),
                IsEmailConfirmed = true,
                IsActive = true
            };

            var customerUser = new User(
                "customer@example.com",
                "John",
                "Customer",
                UserRole.Customer,
                vendorTenant.TenantId
            )
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                IsEmailConfirmed = true,
                IsActive = true
            };

            await context.Users.AddAsync(superAdminUser);
            await context.Users.AddAsync(vendorUser);
            await context.Users.AddAsync(customerUser);

            // Seed Categories
            var electronicsCat = new Category(vendorTenant.TenantId, "Electronics", "electronics")
            {
                DisplayOrder = 1
            };

            var clothingCat = new Category(vendorTenant.TenantId, "Clothing", "clothing")
            {
                DisplayOrder = 2
            };

            await context.Categories.AddAsync(electronicsCat);
            await context.Categories.AddAsync(clothingCat);

            // Seed Products
            var products = new List<Product>
            {
                new Product(vendorTenant.TenantId, electronicsCat.Id, "Wireless Headphones", "wireless-headphones", 79.99m)
                {
                    Description = "High-quality wireless headphones with noise cancellation",
                    DiscountPrice = 59.99m,
                    StockQuantity = 50,
                    SKU = "ELEC-001",
                    Status = ProductStatus.Active,
                    AverageRating = 4.5m
                },
                new Product(vendorTenant.TenantId, electronicsCat.Id, "USB-C Cable", "usb-c-cable", 12.99m)
                {
                    Description = "Durable USB-C charging cable",
                    StockQuantity = 200,
                    SKU = "ELEC-002",
                    Status = ProductStatus.Active,
                    AverageRating = 4.0m
                },
                new Product(vendorTenant.TenantId, clothingCat.Id, "Cotton T-Shirt", "cotton-t-shirt", 24.99m)
                {
                    Description = "Comfortable cotton t-shirt available in multiple colors",
                    DiscountPrice = 19.99m,
                    StockQuantity = 100,
                    SKU = "CLOTH-001",
                    Status = ProductStatus.Active,
                    AverageRating = 4.2m
                }
            };

            await context.Products.AddRangeAsync(products);

            // Seed Theme
            var theme = new Theme(vendorTenant.TenantId, "Modern Blue")
            {
                PrimaryColor = "#3B82F6",
                SecondaryColor = "#10B981",
                IsActive = true,
                ThemeJson = @"{
                    ""fontFamily"": ""Segoe UI"",
                    ""fontSize"": ""16px"",
                    ""layout"": ""standard"",
                    ""headerStyle"": ""modern""
                }"
            };

            await context.Themes.AddAsync(theme);

            // Seed Discount
            var discount = new Discount(vendorTenant.TenantId, "SUMMER20", DiscountType.Percentage, 20)
            {
                Description = "Summer sale discount",
                MaxUses = 100,
                MinOrderAmount = 50
            };

            await context.Discounts.AddAsync(discount);

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to seed database", ex);
        }
    }
}
