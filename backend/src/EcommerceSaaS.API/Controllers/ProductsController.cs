using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceSaaS.Application.DTOs;
using EcommerceSaaS.Infrastructure.Repositories;

namespace EcommerceSaaS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all products for a tenant
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResponseDto<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] Guid tenantId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Fetching products for tenant: {TenantId}, page: {Page}", tenantId, page);

            var products = (await _productRepository.GetByTenantAsync(tenantId, page, pageSize)).ToList();
            var totalCount = await _productRepository.GetCountByTenantAsync(tenantId);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Status = (int)p.Status,
                CategoryId = p.CategoryId,
                AverageRating = p.AverageRating,
                Images = p.Images
            }).ToList();

            var response = new PaginatedResponseDto<ProductDto>
            {
                Data = productDtos,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponseDto<PaginatedResponseDto<ProductDto>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return StatusCode(500, ApiResponseDto<object>.ErrorResponse("Failed to fetch products"));
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound(ApiResponseDto<object>.ErrorResponse("Product not found"));

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            StockQuantity = product.StockQuantity,
            SKU = product.SKU,
            Status = (int)product.Status,
            CategoryId = product.CategoryId,
            AverageRating = product.AverageRating,
            Images = product.Images
        };

        return Ok(ApiResponseDto<ProductDto>.SuccessResponse(productDto));
    }

    /// <summary>
    /// Create a new product (Vendor only)
    /// </summary>
    [Authorize(Roles = "Vendor")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto request, [FromQuery] Guid tenantId)
    {
        try
        {
            var product = new Domain.Entities.Product(tenantId, request.CategoryId, request.Name, request.Slug, request.Price)
            {
                Description = request.Description,
                DiscountPrice = request.DiscountPrice,
                StockQuantity = request.StockQuantity,
                SKU = request.SKU
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                StockQuantity = product.StockQuantity,
                SKU = product.SKU,
                Status = (int)product.Status,
                CategoryId = product.CategoryId
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, 
                ApiResponseDto<ProductDto>.SuccessResponse(productDto, "Product created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return BadRequest(ApiResponseDto<object>.ErrorResponse("Failed to create product"));
        }
    }

    /// <summary>
    /// Update product (Vendor only)
    /// </summary>
    [Authorize(Roles = "Vendor")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto request)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound(ApiResponseDto<object>.ErrorResponse("Product not found"));

        if (!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description)) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.DiscountPrice.HasValue) product.DiscountPrice = request.DiscountPrice.Value;
        if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
        if (request.Status.HasValue) product.Status = (Domain.Enums.ProductStatus)request.Status.Value;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            StockQuantity = product.StockQuantity,
            SKU = product.SKU,
            Status = (int)product.Status,
            CategoryId = product.CategoryId
        };

        return Ok(ApiResponseDto<ProductDto>.SuccessResponse(productDto, "Product updated successfully"));
    }

    /// <summary>
    /// Delete product (Vendor only)
    /// </summary>
    [Authorize(Roles = "Vendor")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return NotFound(ApiResponseDto<object>.ErrorResponse("Product not found"));

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync();

        return Ok(ApiResponseDto<object>.SuccessResponse(null, "Product deleted successfully"));
    }
}
