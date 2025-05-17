using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class CartItemViewModel
    {

        [Required(ErrorMessage = "ProductId is required.")]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "ProductName is required.")]
        public string ProductName { get; set; }
       
    }
}
