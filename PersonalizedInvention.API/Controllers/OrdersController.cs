<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Application.DTOs;
=======
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
>>>>>>> beta

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
<<<<<<< HEAD
=======
    [Authorize]
>>>>>>> beta
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;

<<<<<<< HEAD
        // GET /api/orders/user/1
=======
>>>>>>> beta
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

<<<<<<< HEAD
        // GET /api/orders/5
=======
>>>>>>> beta
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

<<<<<<< HEAD
        // POST /api/orders/checkout/1
        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId)
        {
            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(userId);
=======
        // ← Now accepts delivery address in the body
        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId, [FromBody] DeliveryAddressDto address)
        {
            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(userId, address);
>>>>>>> beta
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
