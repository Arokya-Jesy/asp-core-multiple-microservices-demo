using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderMicroservice.Models;
using OrderMicroservice.Services;
using System.Security.Claims;

namespace OrderMicroservice.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
        var authToken = Request.Headers["Authorization"].FirstOrDefault();
        var order = await _orderService.CreateOrderAsync(request, username, authToken);
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
        var orders = await _orderService.GetUserOrdersAsync(username);
        return Ok(orders);
    }
}
