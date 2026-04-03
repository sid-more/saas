using EcommerceSaaS.Domain.Enums;

namespace EcommerceSaaS.Domain.Entities;

/// <summary>
/// Base entity class for all domain entities
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
}

/// <summary>
/// Tenant entity - represents a vendor's store
/// </summary>
public class Tenant : Entity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string? Logo { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
    public TenantStatus Status { get; set; }
    public int MaxProducts { get; set; }
    public int MaxUsers { get; set; }
    public DateTime? SubscriptionExpiry { get; set; }

    public Tenant(string name, string slug, SubscriptionPlan plan = SubscriptionPlan.Freemium)
    {
        TenantId = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        SubscriptionPlan = plan;
        Status = TenantStatus.Active;
        MaxProducts = plan switch
        {
            SubscriptionPlan.Freemium => 10,
            SubscriptionPlan.Starter => 100,
            SubscriptionPlan.Professional => 1000,
            SubscriptionPlan.Enterprise => int.MaxValue,
            _ => 10
        };
        MaxUsers = plan switch
        {
            SubscriptionPlan.Freemium => 1,
            SubscriptionPlan.Starter => 5,
            SubscriptionPlan.Professional => 20,
            SubscriptionPlan.Enterprise => int.MaxValue,
            _ => 1
        };
    }
}

/// <summary>
/// User entity - handles authentication for all roles
/// </summary>
public class User : Entity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLogin { get; set; }

    public User() { }

    public User(string email, string firstName, string lastName, UserRole role, Guid? tenantId = null)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Role = role;
        TenantId = tenantId ?? Guid.Empty;
        IsActive = true;
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}

/// <summary>
/// Theme configuration entity
/// </summary>
public class Theme : Entity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#000000";
    public string SecondaryColor { get; set; } = "#FFFFFF";
    public string? Logo { get; set; }
    public string? Banner { get; set; }
    public string ThemeJson { get; set; } = "{}";
    public bool IsActive { get; set; }

    public Theme(Guid tenantId, string name)
    {
        TenantId = tenantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsActive = true;
    }
}

/// <summary>
/// Category entity
/// </summary>
public class Category : Entity
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }

    public Category() { }

    public Category(Guid tenantId, string name, string slug)
    {
        TenantId = tenantId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
    }
}

/// <summary>
/// Product entity
/// </summary>
public class Product : Entity
{
    public Guid TenantId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = string.Empty;
    public ProductStatus Status { get; set; }
    public int ViewCount { get; set; }
    public decimal AverageRating { get; set; }
    public List<string> Images { get; set; } = new();

    public Product() { }

    public Product(Guid tenantId, Guid categoryId, string name, string slug, decimal price)
    {
        TenantId = tenantId;
        CategoryId = categoryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Price = price;
        Status = ProductStatus.Draft;
        StockQuantity = 0;
    }

    public bool IsInStock() => StockQuantity > 0;
    
    public decimal GetEffectivePrice() => DiscountPrice ?? Price;
}

/// <summary>
/// Cart entity
/// </summary>
public class Cart : Entity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public List<CartItem> Items { get; set; } = new();

    public Cart() { }

    public Cart(Guid tenantId, Guid userId)
    {
        TenantId = tenantId;
        UserId = userId;
    }

    public void AddItem(CartItem item) => Items.Add(item);
    
    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(x => x.ProductId == productId);
    }

    public decimal GetTotal() => Items.Sum(x => x.Quantity * x.Price);
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string ProductName { get; set; } = string.Empty;
}

/// <summary>
/// Order entity
/// </summary>
public class Order : Entity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }

    public Order() { }

    public Order(Guid tenantId, Guid userId, decimal totalAmount)
    {
        TenantId = tenantId;
        UserId = userId;
        TotalAmount = totalAmount;
        OrderNumber = GenerateOrderNumber();
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentStatus.Pending;
    }

    private static string GenerateOrderNumber() => 
        $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// Review entity
/// </summary>
public class Review : Entity
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public bool IsVerifiedPurchase { get; set; }
    public int HelpfulCount { get; set; }

    public Review() { }

    public Review(Guid tenantId, Guid productId, Guid userId, int rating, string title)
    {
        TenantId = tenantId;
        ProductId = productId;
        UserId = userId;
        Rating = rating > 5 ? 5 : rating < 1 ? 1 : rating;
        Title = title ?? throw new ArgumentNullException(nameof(title));
    }
}

/// <summary>
/// Discount/Coupon entity
/// </summary>
public class Discount : Entity
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MaxDiscount { get; set; }
    public decimal MinOrderAmount { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    public Discount() { }

    public Discount(Guid tenantId, string code, DiscountType type, decimal value)
    {
        TenantId = tenantId;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Type = type;
        Value = value;
        IsActive = true;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddMonths(1);
    }

    public bool IsValid() => 
        IsActive && 
        DateTime.UtcNow >= StartDate && 
        DateTime.UtcNow <= EndDate && 
        UsedCount < MaxUses;
}
