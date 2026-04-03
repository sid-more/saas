using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using EcommerceSaaS.Domain.Entities;
using EcommerceSaaS.Domain.Enums;
using EcommerceSaaS.Infrastructure.Repositories;

namespace EcommerceSaaS.Infrastructure.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, User? User)> RegisterAsync(string email, string firstName, string lastName, string password);
    Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> LoginAsync(string email, string password);
    Task<(bool Success, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken);
    string? ValidateToken(string token);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "EcommerceSaaS";
        _jwtAudience = configuration["Jwt:Audience"] ?? "EcommerceSaaSUsers";
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(
        string email, string firstName, string lastName, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required", null);

            // For now, SuperAdmin tenant registration - in production, use onboarded tenant
            var superAdminTenantId = Guid.Empty;

            var existingUser = await _userRepository.GetByEmailAsync(email, superAdminTenantId);
            if (existingUser != null)
                return (false, "Email already registered", null);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User(email, firstName, lastName, UserRole.Customer, superAdminTenantId)
            {
                PasswordHash = passwordHash,
                IsEmailConfirmed = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Registration successful", user);
        }
        catch (Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> LoginAsync(
        string email, string password)
    {
        try
        {
            var superAdminTenantId = Guid.Empty;
            var user = await _userRepository.GetByEmailAsync(email, superAdminTenantId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, "Invalid email or password", null, null);

            if (!user.IsActive)
                return (false, "Account is inactive", null, null);

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Login successful", accessToken, refreshToken);
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null, null);
        }
    }

    public async Task<(bool Success, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var superAdminTenantId = Guid.Empty;
            var users = await _userRepository.GetByTenantAsync(superAdminTenantId);
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return (false, null, null);

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return (true, newAccessToken, newRefreshToken);
        }
        catch
        {
            return (false, null, null);
        }
    }

    public string? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
        catch
        {
            return null;
        }
    }

    private string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.GetFullName()),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

public interface IProductService
{
    Task<(bool Success, string Message, Domain.Entities.Product? Product)> CreateProductAsync(
        Guid tenantId, string name, string slug, decimal price, Guid categoryId);
    Task<(bool Success, Domain.Entities.Product? Product)> GetProductAsync(Guid tenantId, Guid productId);
    Task<(bool Success, List<Domain.Entities.Product> Products)> GetProductsByTenantAsync(Guid tenantId, int page, int pageSize);
    Task<(bool Success, string Message)> UpdateProductAsync(Guid productId, string? name, decimal? price, int? stock);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<(bool Success, string Message, Domain.Entities.Product? Product)> CreateProductAsync(
        Guid tenantId, string name, string slug, decimal price, Guid categoryId)
    {
        try
        {
            var product = new Domain.Entities.Product(tenantId, categoryId, name, slug, price);
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return (true, "Product created successfully", product);
        }
        catch (Exception ex)
        {
            return (false, $"Failed to create product: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, Domain.Entities.Product? Product)> GetProductAsync(Guid tenantId, Guid productId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product?.TenantId != tenantId)
                return (false, null);
            return (true, product);
        }
        catch
        {
            return (false, null);
        }
    }

    public async Task<(bool Success, List<Domain.Entities.Product> Products)> GetProductsByTenantAsync(
        Guid tenantId, int page, int pageSize)
    {
        try
        {
            var products = (await _productRepository.GetByTenantAsync(tenantId, page, pageSize)).ToList();
            return (true, products);
        }
        catch
        {
            return (false, new List<Domain.Entities.Product>());
        }
    }

    public async Task<(bool Success, string Message)> UpdateProductAsync(Guid productId, string? name, decimal? price, int? stock)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Product not found");

            if (!string.IsNullOrEmpty(name)) product.Name = name;
            if (price.HasValue) product.Price = price.Value;
            if (stock.HasValue) product.StockQuantity = stock.Value;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return (true, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Update failed: {ex.Message}");
        }
    }
}
