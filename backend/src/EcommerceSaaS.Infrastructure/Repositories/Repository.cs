using Microsoft.EntityFrameworkCore;
using EcommerceSaaS.Domain.Entities;
using EcommerceSaaS.Infrastructure.Persistence;

namespace EcommerceSaaS.Infrastructure.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetBySlugAsync(string slug);
    Task<Tenant?> GetByDomainAsync(string domain);
    Task<IEnumerable<Tenant>> GetByStatusAsync(int status);
}

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Tenant?> GetBySlugAsync(string slug)
    {
        return await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug.ToLower());
    }

    public async Task<Tenant?> GetByDomainAsync(string domain)
    {
        return await _context.Tenants.FirstOrDefaultAsync(t => t.Domain == domain.ToLower());
    }

    public async Task<IEnumerable<Tenant>> GetByStatusAsync(int status)
    {
        return await _context.Tenants.Where(t => (int)t.Status == status).ToListAsync();
    }
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, Guid tenantId);
    Task<IEnumerable<User>> GetByTenantAsync(Guid tenantId);
    Task<IEnumerable<User>> GetByRoleAsync(int role, Guid tenantId);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, Guid tenantId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
    }

    public async Task<IEnumerable<User>> GetByTenantAsync(Guid tenantId)
    {
        return await _context.Users.Where(u => u.TenantId == tenantId).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(int role, Guid tenantId)
    {
        return await _context.Users.Where(u => u.TenantId == tenantId && (int)u.Role == role).ToListAsync();
    }
}

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySlugAsync(string slug, Guid tenantId);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, Guid tenantId);
    Task<IEnumerable<Product>> GetByTenantAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<int> GetCountByTenantAsync(Guid tenantId);
}

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetBySlugAsync(string slug, Guid tenantId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Slug == slug && p.TenantId == tenantId);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, Guid tenantId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId && p.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByTenantAsync(Guid tenantId, int pageNumber, int pageSize)
    {
        return await _context.Products
            .Where(p => p.TenantId == tenantId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByTenantAsync(Guid tenantId)
    {
        return await _context.Products.CountAsync(p => p.TenantId == tenantId);
    }
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, Guid tenantId);
    Task<IEnumerable<Order>> GetByUserAsync(Guid userId, Guid tenantId);
    Task<IEnumerable<Order>> GetByTenantAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<decimal> GetTotalRevenueAsync(Guid tenantId);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, Guid tenantId)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber && o.TenantId == tenantId);
    }

    public async Task<IEnumerable<Order>> GetByUserAsync(Guid userId, Guid tenantId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId && o.TenantId == tenantId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByTenantAsync(Guid tenantId, int pageNumber, int pageSize)
    {
        return await _context.Orders
            .Where(o => o.TenantId == tenantId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(Guid tenantId)
    {
        return await _context.Orders
            .Where(o => o.TenantId == tenantId && (int)o.PaymentStatus == 2) // Completed
            .SumAsync(o => o.TotalAmount);
    }
}

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, Guid tenantId);
    Task<IEnumerable<Category>> GetByTenantAsync(Guid tenantId);
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Category?> GetBySlugAsync(string slug, Guid tenantId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.TenantId == tenantId);
    }

    public async Task<IEnumerable<Category>> GetByTenantAsync(Guid tenantId)
    {
        return await _context.Categories
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }
}

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByProductAsync(Guid productId, Guid tenantId);
    Task<Review?> GetByProductAndUserAsync(Guid productId, Guid userId);
}

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetByProductAsync(Guid productId, Guid tenantId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId && r.TenantId == tenantId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByProductAndUserAsync(Guid productId, Guid userId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
    }
}

public interface IDiscountRepository : IRepository<Discount>
{
    Task<Discount?> GetByCodeAsync(string code, Guid tenantId);
    Task<IEnumerable<Discount>> GetActiveByTenantAsync(Guid tenantId);
}

public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Discount?> GetByCodeAsync(string code, Guid tenantId)
    {
        return await _context.Discounts
            .FirstOrDefaultAsync(d => d.Code == code.ToUpper() && d.TenantId == tenantId);
    }

    public async Task<IEnumerable<Discount>> GetActiveByTenantAsync(Guid tenantId)
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Where(d => d.TenantId == tenantId && 
                       d.IsActive && 
                       d.StartDate <= now && 
                       d.EndDate >= now)
            .ToListAsync();
    }
}
