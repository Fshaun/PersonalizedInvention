using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Application.DTOs
{
    public class CreatePaymentIntentDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "zar";
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
