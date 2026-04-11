using PersonalizedInvention.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntentResponseDto> CreatePaymentIntentAsync(CreatePaymentIntentDto dto);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId, int orderId);
    }
}
