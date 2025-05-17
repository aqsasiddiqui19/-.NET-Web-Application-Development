using Ecommerce_API_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class OrderItemViewModel
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total price must be non-negative.")]
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }

    }
}
