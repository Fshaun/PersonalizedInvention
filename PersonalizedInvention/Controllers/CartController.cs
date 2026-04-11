using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService) => _cartService = cartService;

        // GET /api/cart/1  (userId = 1 for now; replace with JWT claims later)
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var items = await _cartService.GetCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            return Ok(new { items, total });
        }

        // POST /api/cart/1/add
        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddItem(int userId, [FromBody] AddToCartDto dto)
        {
            var item = await _cartService.AddItemAsync(userId, dto);
            if (item is null)
                return BadRequest(new { message = "Product not available or insufficient stock." });
            return Ok(item);
        }

        // PUT /api/cart/1/update
        [HttpPut("{userId}/update")]
        public async Task<IActionResult> UpdateItem(int userId, [FromBody] UpdateCartItemDto dto)
        {
            var item = await _cartService.UpdateItemAsync(userId, dto);
            return Ok(item);
        }

        // DELETE /api/cart/1/remove/5
        [HttpDelete("{userId}/remove/{productId}")]
        public async Task<IActionResult> RemoveItem(int userId, int productId)
        {
            var success = await _cartService.RemoveItemAsync(userId, productId);
            return success ? NoContent() : NotFound();
        }

        // DELETE /api/cart/1/clear
        [HttpDelete("{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
