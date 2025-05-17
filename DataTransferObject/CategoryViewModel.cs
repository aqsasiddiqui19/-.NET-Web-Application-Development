using System.ComponentModel.DataAnnotations;
using Ecommerce_API_Project.Models;
using Newtonsoft.Json;


namespace Ecommerce_API_Project.DataTransferObject
{
    public class CategoryViewModel
    {

        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string CategoryDescription { get; set; }

        [Required]
        public Guid AdminId { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

    }
}

