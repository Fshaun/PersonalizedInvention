using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        // ← Now accepts delivery address in the body
        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId, [FromBody] DeliveryAddressDto address)
        {
            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(userId, address);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
