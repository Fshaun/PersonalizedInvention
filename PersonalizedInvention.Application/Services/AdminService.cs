using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Enums;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public AdminService(IProductRepository productRepo, IOrderRepository orderRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        // ── Products ──────────────────────────────────────────────────

        public async Task<IEnumerable<AdminProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(MapProductToDto);
        }

        public async Task<AdminProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _productRepo.CreateAsync(product);
            return MapProductToDto(created);
        }

        public async Task<AdminProductDto?> UpdateProductAsync(int id, CreateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product is null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.Category = dto.Category;

            var updated = await _productRepo.UpdateAsync(product);
            return MapProductToDto(updated);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product is null) return false;
            await _productRepo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateStockAsync(int id, int stock)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product is null) return false;
            product.Stock = stock;
            await _productRepo.UpdateAsync(product);
            return true;
        }

        // ── Orders ────────────────────────────────────────────────────

        public async Task<IEnumerable<AdminOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            return orders.Select(MapOrderToDto);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, out var parsedStatus))
                return false;

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order is null) return false;

            order.Status = parsedStatus;
            await _orderRepo.UpdateAsync(order);
            return true;
        }

        // ── Mappers ───────────────────────────────────────────────────

        private static AdminProductDto MapProductToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            Category = p.Category,
            IsInStock = p.IsInStock,
            CreatedAt = p.CreatedAt
        };

        private static AdminOrderDto MapOrderToDto(Order o) => new()
        {
            Id = o.Id,
            UserId = o.UserId,
            UserEmail = o.User?.Email ?? string.Empty,
            UserFullName = o.User?.FullName ?? string.Empty,
            TotalAmount = o.TotalAmount,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt,
            OrderItems = o.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product?.Name ?? string.Empty,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}
