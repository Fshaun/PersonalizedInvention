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

<<<<<<< HEAD
        public async Task<OrderDto> CreateOrderFromCartAsync(int userId)
        {
            // Step 1: get cart items
=======
        public async Task<OrderDto> CreateOrderFromCartAsync(int userId, DeliveryAddressDto address)
        {
>>>>>>> beta
            var cartItems = (await _cartRepository.GetCartItemsByUserIdAsync(userId)).ToList();
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

<<<<<<< HEAD
            // Step 2: build order items and calculate total
=======
>>>>>>> beta
            var orderItems = cartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
<<<<<<< HEAD
                UnitPrice = ci.Product.Price  // Lock price at time of order
=======
                UnitPrice = ci.Product.Price
>>>>>>> beta
            }).ToList();

            var total = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

<<<<<<< HEAD
            // Step 3: create the order
=======
>>>>>>> beta
            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = OrderStatus.Pending,
<<<<<<< HEAD
                OrderItems = orderItems
            };

            var created = await _orderRepository.CreateAsync(order);

            // Step 4: clear the cart after order is placed
            await _cartRepository.ClearCartAsync(userId);

=======
                OrderItems = orderItems,

                // ── Save delivery address ──────────────────────
                DeliveryFullName = address.FullName,
                DeliveryPhone = address.Phone,
                DeliveryStreet = address.Street,
                DeliveryCity = address.City,
                DeliveryProvince = address.Province,
                DeliveryPostalCode = address.PostalCode,
                DeliveryCountry = address.Country
            };

            var created = await _orderRepository.CreateAsync(order);
            await _cartRepository.ClearCartAsync(userId);
>>>>>>> beta
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
<<<<<<< HEAD
=======
            DeliveryAddress = new DeliveryAddressDto
            {
                FullName = o.DeliveryFullName,
                Phone = o.DeliveryPhone,
                Street = o.DeliveryStreet,
                City = o.DeliveryCity,
                Province = o.DeliveryProvince,
                PostalCode = o.DeliveryPostalCode,
                Country = o.DeliveryCountry
            },
>>>>>>> beta
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
