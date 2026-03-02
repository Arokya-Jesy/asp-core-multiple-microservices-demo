using OrderMicroservice.Models;
using OrderMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderMicroservice.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request, string username, string? authToken);
    Task<List<Order>> GetUserOrdersAsync(string username);
}

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderService> _logger;
    private readonly string _productServiceUrl;

    public OrderService(OrderDbContext context, HttpClient httpClient, ILogger<OrderService> logger, IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _logger = logger;
        _productServiceUrl = configuration["ServiceUrls:ProductService"] ?? throw new InvalidOperationException("ProductService URL not configured");
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, string username, string? authToken)
    {
        // Inter-service communication: Call Product Service
        _logger.LogInformation($"Calling Product Service for product {request.ProductId}");
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_productServiceUrl}/api/products/{request.ProductId}");
        if (!string.IsNullOrEmpty(authToken))
        {
            requestMessage.Headers.Add("Authorization", authToken);
        }
        
        var response = await _httpClient.SendAsync(requestMessage);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Product not found");
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        
        if (product == null)
        {
            throw new Exception("Failed to deserialize product");
        }

        var order = new Order
        {
            ProductId = product.Id,
            ProductName = product.Name,
            ProductPrice = product.Price,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            OrderDate = DateTime.UtcNow,
            Username = username
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Order created: {order.Id} for user {username}");
        
        return order;
    }

    public async Task<List<Order>> GetUserOrdersAsync(string username)
    {
        return await _context.Orders
            .Where(o => o.Username == username)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
