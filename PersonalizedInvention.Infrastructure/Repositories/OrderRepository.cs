using Microsoft.EntityFrameworkCore;
using PersonalizedInvention.Infrastructure.Data;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId) =>
            await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)  // Nested JOIN
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<Order?> GetByIdAsync(int id) =>
            await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
