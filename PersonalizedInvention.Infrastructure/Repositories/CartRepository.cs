using Microsoft.EntityFrameworkCore;
using PersonalizedInvention.Infrastructure.Data;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<CartItem>> GetCartItemsByUserIdAsync(int userId) =>
            await _context.CartItems
                .Include(ci => ci.Product)   // JOIN with Products table
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

        public async Task<CartItem?> GetCartItemAsync(int userId, int productId) =>
            await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        public async Task<CartItem> AddAsync(CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
            await _context.Entry(item).Reference(ci => ci.Product).LoadAsync();
            return item;
        }

        public async Task<CartItem> UpdateAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
            await _context.Entry(item).Reference(ci => ci.Product).LoadAsync();
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item is not null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
