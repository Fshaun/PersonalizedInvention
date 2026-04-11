using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using PersonalizedInvention.Domain.Enums;

namespace PersonalizedInvention.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderFromCartAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
