using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.Models
{
   public class Admin
    {
        public Guid AdminId { get; set; } = Guid.NewGuid(); 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Admin";
        public bool? IsActive { get; set; } = true;
    }
}

