using Ecommerce_API_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_API_Project.Models
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 4)]
        public decimal FinalAmount { get; set; }

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}

