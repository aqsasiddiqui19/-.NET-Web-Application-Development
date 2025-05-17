using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class PriceViewModel
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal BasePrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount Percentage must be between 0 and 100.")]
        public decimal TaxPercentage { get; set; } = 0m; // Default to 0

        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public decimal DiscountPercentage { get; set; } = 0m; // Default to 0

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // NULL means it's the current price

        [Required]
        public Guid AdminId { get; set; }
    }
}
