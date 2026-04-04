using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EcommerceSaaS.Domain.Entities;
using EcommerceSaaS.Domain.Enums;
using System.Linq.Expressions;

namespace EcommerceSaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly Guid? _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, Guid? tenantId) 
        : base(options)
    {
        _tenantId = tenantId;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTenant(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureTheme(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureProduct(modelBuilder);
        ConfigureCart(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureReview(modelBuilder);
        ConfigureDiscount(modelBuilder);

        // Global query filter for soft deletes
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(Entity).IsAssignableFrom(e.ClrType));

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(BuildIsDeletedFilter(entityType.ClrType));
        }
    }

    private static LambdaExpression BuildIsDeletedFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(Entity.IsDeleted));
        var notProperty = Expression.Not(property);
        return Expression.Lambda(notProperty, parameter);
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TenantId).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SubscriptionPlan).HasConversion<int>();
            entity.Property(e => e.Status).HasConversion<int>();
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<int>();
        });
    }

    private static void ConfigureTheme(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.IsActive });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PrimaryColor).HasMaxLength(7);
            entity.Property(e => e.SecondaryColor).HasMaxLength(7);
            entity.Property(e => e.ThemeJson).HasColumnType("jsonb");
        });
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Slug }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.DiscountPrice).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Images).HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            );
        });
    }

    private static void ConfigureCart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
            entity.OwnsMany(e => e.Items);
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.PaymentStatus).HasConversion<int>();
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.ShippingAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.OwnsMany(e => e.Items);
        });
    }

    private static void ConfigureReview(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.ProductId });
            entity.HasIndex(e => new { e.ProductId, e.UserId }).IsUnique();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Comment).HasMaxLength(2000);
        });
    }

    private static void ConfigureDiscount(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).HasConversion<int>();
            entity.Property(e => e.Value).HasPrecision(18, 2);
            entity.Property(e => e.MaxDiscount).HasPrecision(18, 2);
            entity.Property(e => e.MinOrderAmount).HasPrecision(18, 2);
        });
    }
}
