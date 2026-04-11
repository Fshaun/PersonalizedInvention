using PersonalizedInvention.Application.DTOs;
using PersonalizedInvention.Application.Interfaces;
using PersonalizedInvention.Domain.Enums;
using PersonalizedInvention.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Stripe;

namespace PersonalizedInvention.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IConfiguration configuration, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(CreatePaymentIntentDto dto)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(dto.Amount * 100),  // Stripe works in cents/smallest currency unit
                Currency = dto.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
            {
                { "OrderId", dto.OrderId.ToString() }
            }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new PaymentIntentResponseDto
            {
                ClientSecret = intent.ClientSecret,
                PaymentIntentId = intent.Id
            };
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null) return false;

            order.StripePaymentIntentId = paymentIntentId;
            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order);
            return true;
        }
    }
}
