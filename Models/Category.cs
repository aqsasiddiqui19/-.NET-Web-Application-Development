using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecommerce_API_Project.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string? ImageUrl { get; set; } // Store the image path or URL
        public Guid AdminId { get; set; }        // FK to Admin
        public List<Product> Products { get; set; } = new List<Product>();
    }
}




//Note:
// Entity Model → Represents the database structure (EF Core model).
// DTO(Data Transfer Object) / ViewModel → Represents the API response format (for frontend).
//ViewModel (if using MVC) → Represents the data used in views (for UI).