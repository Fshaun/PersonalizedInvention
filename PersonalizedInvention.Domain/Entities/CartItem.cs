using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalizedInvention.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }

        // Navigation properties — lets EF Core JOIN tables automatically
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }

}
