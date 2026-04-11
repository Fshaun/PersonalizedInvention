using PersonalizedInvention.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task<CartItem> AddAsync(CartItem item);
        Task<CartItem> UpdateAsync(CartItem item);
        Task DeleteAsync(int id);
        Task ClearCartAsync(int userId);
    }
}
