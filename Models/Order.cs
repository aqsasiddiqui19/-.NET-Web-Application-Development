using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_API_Project.Models
{
    public class Order //after checkout process
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid CartId { get; set; } // Track which cart this order came from
        public string Email { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Precision(18, 4)]
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }   
        public Guid ShippingId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}

