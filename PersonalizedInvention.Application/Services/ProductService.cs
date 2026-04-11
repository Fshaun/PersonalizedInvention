using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Services
{
    // SOLID — Single Responsibility: this class only handles product business logic
    // SOLID — Dependency Inversion: depends on IProductRepository interface, not the concrete class
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);  // Map entities to DTOs
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetInStockProductsAsync()
        {
            var products = await _productRepository.GetInStockAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category
            };
            var created = await _productRepository.CreateAsync(product);
            return MapToDto(created);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, CreateProductDto dto)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing is null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.Stock = dto.Stock;
            existing.ImageUrl = dto.ImageUrl;
            existing.Category = dto.Category;

            var updated = await _productRepository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing is null) return false;
            await _productRepository.DeleteAsync(id);
            return true;
        }

        // Private mapper — keeps mapping logic in one place (SRP)
        private static ProductDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            Category = p.Category,
            IsInStock = p.IsInStock
        };
    }
}
