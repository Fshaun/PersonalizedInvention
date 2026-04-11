using PersonalizedInvention.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
    }
}
