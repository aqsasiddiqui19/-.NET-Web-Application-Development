using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ecommerce_API_Project.Models
{
    public class Brand
    {
        [Key]
        public Guid BrandId { get; set; } = Guid.NewGuid();
        public string BrandName { get; set; }
        public string? BrandDescription { get; set; }
        public Guid AdminId { get; set; }   // FK to Admin
    }
}











//Note:
//string----Non-nullable reference type-----Use when the string is required
//string?---Nullable reference type--------- Use when the string is optional / can be null
//string.Empty----Constant value ("")--------Use when you want an empty string (not null)