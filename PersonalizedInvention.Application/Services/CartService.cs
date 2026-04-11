using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartAsync(int userId)
        {
            var items = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            return items.Select(MapToDto);
        }

        public async Task<CartItemDto?> AddItemAsync(int userId, AddToCartDto dto)
        {
            // Business rule: validate product exists and has enough stock
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product is null || product.Stock < dto.Quantity) return null;

            var existing = await _cartRepository.GetCartItemAsync(userId, dto.ProductId);

            if (existing is not null)
            {
                // Business rule: if already in cart, increment quantity
                existing.Quantity += dto.Quantity;
                var updated = await _cartRepository.UpdateAsync(existing);
                return MapToDto(updated);
            }

            var newItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            var created = await _cartRepository.AddAsync(newItem);
            return MapToDto(created);
        }

        public async Task<CartItemDto?> UpdateItemAsync(int userId, UpdateCartItemDto dto)
        {
            var item = await _cartRepository.GetCartItemAsync(userId, dto.ProductId);
            if (item is null) return null;

            // Business rule: quantity of 0 means remove the item
            if (dto.Quantity <= 0)
            {
                await _cartRepository.DeleteAsync(item.Id);
                return null;
            }

            item.Quantity = dto.Quantity;
            var updated = await _cartRepository.UpdateAsync(item);
            return MapToDto(updated);
        }

        public async Task<bool> RemoveItemAsync(int userId, int productId)
        {
            var item = await _cartRepository.GetCartItemAsync(userId, productId);
            if (item is null) return false;
            await _cartRepository.DeleteAsync(item.Id);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            await _cartRepository.ClearCartAsync(userId);
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var items = await _cartRepository.GetCartItemsByUserIdAsync(userId);
            return items.Sum(i => i.Product.Price * i.Quantity);
        }

        private static CartItemDto MapToDto(CartItem item) => new()
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            ProductImageUrl = item.Product?.ImageUrl ?? string.Empty,
            UnitPrice = item.Product?.Price ?? 0,
            Quantity = item.Quantity
        };
    }
}
