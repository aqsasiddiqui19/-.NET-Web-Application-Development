using Ecommerce_API_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class OrderViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total items must be at least 1.")]
        public int TotalItems { get; set; } 

        [Required]
        public string OrderStatus { get; set; } 
    
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required in the order.")]
        public List<OrderItem> OrderItems { get; set; }

    }
}

















//[RegularExpression("Pending|Processing|Shipped|Delivered|Cancelled",
//    = "Example: Pending, Processing, Shipped, Delivered, Cancelled.\r\n\r\n";
//   ErrorMessage = "Order status must be: Pending, Processing, Shipped, Delivered, or Cancelled.")]