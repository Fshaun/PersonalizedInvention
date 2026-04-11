using Microsoft.AspNetCore.Mvc;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Application.DTOs;

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Route: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        // Constructor injection — C# automatically provides IProductService
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET /api/products/instock
        [HttpGet("instock")]
        public async Task<IActionResult> GetInStock()
        {
            var products = await _productService.GetInStockProductsAsync();
            return Ok(products);
        }

        // GET /api/products/category/Clothing
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        // GET /api/products/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        // POST /api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT /api/products/3
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            return product is null ? NotFound() : Ok(product);
        }

        // DELETE /api/products/3
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
