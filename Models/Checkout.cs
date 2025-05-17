using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_API_Project.Models
{
    public class Checkout
    {
        [Key]
        public Guid CheckoutId { get; set; } = Guid.NewGuid();
        public Guid CartId { get; set; } // Optional: reference from session-based cart

        [Required]
        [EmailAddress]
        public string Email { get; set; }  // Contact Info

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public Guid ShippingId { get; set; }

    }
}
