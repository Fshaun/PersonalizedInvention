using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   // JWT required — we check IsAdmin in each action
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
            => _adminService = adminService;

        // Helper — reads isAdmin claim from the JWT token
        private bool IsAdmin()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "isAdmin");
            return claim?.Value == "true";
        }

        // ── Products ──────────────────────────────────────────────────

        // GET /api/admin/products
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!IsAdmin()) return Forbid();
            var products = await _adminService.GetAllProductsAsync();
            return Ok(products);
        }

        // POST /api/admin/products
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!IsAdmin()) return Forbid();
            var product = await _adminService.CreateProductAsync(dto);
            return Ok(product);
        }

        // PUT /api/admin/products/3
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDto dto)
        {
            if (!IsAdmin()) return Forbid();
            var product = await _adminService.UpdateProductAsync(id, dto);
            return product is null ? NotFound() : Ok(product);
        }

        // DELETE /api/admin/products/3
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin()) return Forbid();
            var success = await _adminService.DeleteProductAsync(id);
            return success ? NoContent() : NotFound();
        }

        // PATCH /api/admin/products/3/stock
        [HttpPatch("products/{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            if (!IsAdmin()) return Forbid();
            var success = await _adminService.UpdateStockAsync(id, dto.Stock);
            return success ? Ok() : NotFound();
        }

        // ── Orders ────────────────────────────────────────────────────

        // GET /api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            if (!IsAdmin()) return Forbid();
            var orders = await _adminService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // PATCH /api/admin/orders/5/status
        [HttpPatch("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!IsAdmin()) return Forbid();
            var success = await _adminService.UpdateOrderStatusAsync(id, dto.Status);
            return success ? Ok() : BadRequest(new { message = "Invalid status or order not found." });
        }
    }
}
