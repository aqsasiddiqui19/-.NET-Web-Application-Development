using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.Models
{
     public class CartItem
     { 
        [Key]
        public Guid CartItemId { get; set; } = Guid.NewGuid();  
        public string ProductName { get; set; }
        public int Quantity { get; set; } = 1;

        [Precision(18, 4)]
        public decimal FinalAmount { get; set; }
        public Guid CartId { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public Product Product { get; set; }
    }
}



