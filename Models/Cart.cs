using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Ecommerce_API_Project.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } = Guid.NewGuid();
        public DateTime? CreatedAt { get; set; }

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }  

    }
}

