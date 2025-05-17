using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce_API_Project.Models
{
    public class Shipping
    {
        [Key]
        public Guid ShippingId { get; set; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        [Required]
        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }
       
        [Required]
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }

        [ForeignKey("user")]
        public Guid UserId { get; set; }    
        public User user { get; set; }  

    }
}
