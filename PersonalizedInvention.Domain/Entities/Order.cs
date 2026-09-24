using System;
using System.Collections.Generic;
using System.Text;
using PersonalizedInvention.Domain.Enums;

namespace PersonalizedInvention.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // ── Delivery address ──────────────────────────────────────
        public string DeliveryFullName { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public string DeliveryStreet { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string DeliveryProvince { get; set; } = string.Empty;
        public string DeliveryPostalCode { get; set; } = string.Empty;
        public string DeliveryCountry { get; set; } = "South Africa";

        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
