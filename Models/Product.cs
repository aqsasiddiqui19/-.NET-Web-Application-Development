using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string? ImageUrl { get; set; } // Store the image path or URL
        public string CategoryName { get; set; }

        [Precision(18, 4)]
        public decimal ProductStock { get; set; }

        [Precision(18, 4)]
        public decimal TaxPercentage { get; set; }

        [Precision(18, 4)]
        public decimal DiscountPercentage { get; set; }

        [Precision(18, 4)]
        public decimal DiscountedPrice { get; set; }

        [Precision(18, 4)]
        public decimal FinalAmount { get; set; }
        public Guid AdminId { get; set; }  // FK to Admin
        public Guid CategoryId { get; set; }
        public Guid PriceId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
