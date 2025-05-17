using Ecommerce_API_Project.Models;
using Ecommerce_API_Project.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class CheckoutViewModel
    {
        [Required]
        [EmailValidator]
        public string Email { get; set; }

        // Shipping Info
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
        public string PostalCode { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Province { get; set; }
    }
}

