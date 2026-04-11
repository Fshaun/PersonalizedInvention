using PersonalizedInvention.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDto>> GetCartAsync(int userId);
        Task<CartItemDto?> AddItemAsync(int userId, AddToCartDto dto);
        Task<CartItemDto?> UpdateItemAsync(int userId, UpdateCartItemDto dto);
        Task<bool> RemoveItemAsync(int userId, int productId);
        Task<bool> ClearCartAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}
