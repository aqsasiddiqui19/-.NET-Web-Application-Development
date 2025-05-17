using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class ProductViewModel
    {

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string ProductDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int ProductStock { get; set; }

        [Required]
        public Guid AdminId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "CategoryName is required")]
        public string CategoryName { get; set; }

        [Required]
        public Guid PriceId { get; set; }
    }
}

