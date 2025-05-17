using Ecommerce_API_Project.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class AdminViewModel
    { 
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailValidator]
        public string Email { get; set; }

        [PasswordValidator] //custom datanotation validate password
        public string? Password { get; set; }
    }
}
