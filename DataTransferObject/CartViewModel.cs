using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Ecommerce_API_Project.Models;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class CartViewModel
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}


