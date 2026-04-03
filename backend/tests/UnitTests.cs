using EcommerceSaaS.Infrastructure.Services;
using Xunit;
using Moq;
using EcommerceSaaS.Infrastructure.Repositories;
using EcommerceSaaS.Domain.Entities;
using EcommerceSaaS.Domain.Enums;

namespace EcommerceSaaS.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Setup JWT configuration
        _mockConfiguration.Setup(x => x["Jwt:Secret"])
            .Returns("your-super-secret-key-min-32-characters-long-for-hs256");
        _mockConfiguration.Setup(x => x["Jwt:Issuer"])
            .Returns("EcommerceSaaS");
        _mockConfiguration.Setup(x => x["Jwt:Audience"])
            .Returns("EcommerceSaaSUsers");

        _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var password = "SecurePass123!";

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<Guid>()))
            .ReturnsAsync((User)null);

        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var (success, message, user) = await _authService.RegisterAsync(email, firstName, lastName, password);

        // Assert
        Assert.True(success);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var email = "existing@example.com";
        var existingUser = new User(email, "Jane", "Doe", UserRole.Customer);

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<Guid>()))
            .ReturnsAsync(existingUser);

        // Act
        var (success, message, user) = await _authService.RegisterAsync(email, "John", "Doe", "Pass123!");

        // Assert
        Assert.False(success);
        Assert.Null(user);
        Assert.Contains("already registered", message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var email = "test@example.com";
        var password = "SecurePass123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User(email, "John", "Doe", UserRole.Customer)
        {
            PasswordHash = passwordHash,
            IsActive = true
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<Guid>()))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var (success, message, accessToken, refreshToken) = await _authService.LoginAsync(email, password);

        // Assert
        Assert.True(success);
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange
        var email = "test@example.com";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPass123!");

        var user = new User(email, "John", "Doe", UserRole.Customer)
        {
            PasswordHash = passwordHash,
            IsActive = true
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<Guid>()))
            .ReturnsAsync(user);

        // Act
        var (success, message, accessToken, refreshToken) = await _authService.LoginAsync(email, "WrongPass123!");

        // Assert
        Assert.False(success);
        Assert.Null(accessToken);
    }
}

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockProductRepository.Object);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ShouldReturnProduct()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var name = "Test Product";
        var slug = "test-product";
        var price = 99.99m;

        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Product>()))
            .Returns(Task.CompletedTask);

        _mockProductRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var (success, message, product) = await _productService.CreateProductAsync(tenantId, name, slug, price, categoryId);

        // Assert
        Assert.True(success);
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        Assert.Equal(slug, product.Slug);
        Assert.Equal(price, product.Price);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Domain.Entities.Product(Guid.NewGuid(), Guid.NewGuid(), "Old Name", "old-slug", 50m);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _mockProductRepository.Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var (success, message) = await _productService.UpdateProductAsync(productId, "New Name", 99.99m, 100);

        // Assert
        Assert.True(success);
        Assert.Equal("New Name", existingProduct.Name);
        Assert.Equal(99.99m, existingProduct.Price);
        Assert.Equal(100, existingProduct.StockQuantity);
    }

    [Fact]
    public async Task GetProductAsync_WithNonExistentProduct_ShouldReturnFalse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Domain.Entities.Product)null);

        // Act
        var (success, product) = await _productService.GetProductAsync(tenantId, productId);

        // Assert
        Assert.False(success);
        Assert.Null(product);
    }
}
