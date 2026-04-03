namespace EcommerceSaaS.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 1,
    Vendor = 2,
    Customer = 3
}

public enum SubscriptionPlan
{
    Freemium = 1,
    Starter = 2,
    Professional = 3,
    Enterprise = 4
}

public enum TenantStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Deleted = 4
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5
}

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    OutOfStock = 3,
    Archived = 4
}

public enum DiscountType
{
    Percentage = 1,
    FixedAmount = 2,
    FreeShipping = 3
}
