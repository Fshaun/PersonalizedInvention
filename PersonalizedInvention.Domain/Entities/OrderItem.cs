using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   // Price at time of purchase

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
