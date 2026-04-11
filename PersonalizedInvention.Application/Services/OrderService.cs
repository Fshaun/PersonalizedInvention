using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Entities;
using PersonalizedInvention.Domain.Interfaces;
using PersonalizedInvention.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order is null ? null : MapToDto(order);
        }

        public async Task<OrderDto> CreateOrderFromCartAsync(int userId)
        {
            // Step 1: get cart items
            var cartItems = (await _cartRepository.GetCartItemsByUserIdAsync(userId)).ToList();
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            // Step 2: build order items and calculate total
            var orderItems = cartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.Price  // Lock price at time of order
            }).ToList();

            var total = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            // Step 3: create the order
            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                OrderItems = orderItems
            };

            var created = await _orderRepository.CreateAsync(order);

            // Step 4: clear the cart after order is placed
            await _cartRepository.ClearCartAsync(userId);

            return MapToDto(created);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null) return false;
            order.Status = status;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private static OrderDto MapToDto(Order o) => new()
        {
            Id = o.Id,
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
