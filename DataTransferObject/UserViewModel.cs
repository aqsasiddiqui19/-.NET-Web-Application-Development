using System.ComponentModel.DataAnnotations;
using Ecommerce_API_Project.Validation;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class UserVeiwModel
    {
        [Required(ErrorMessage = "Username is Required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is Required")]
        [EmailValidator]
        public string Email { get; set; } = string.Empty;

        [PasswordValidator] //custom datanotation validate password
        public string? Password { get; set; }
    }
}



















//USER DATA WILL BE VALIDATED USING USERVIEWMODEL
//DataNotation model ko validate karne ke liye use kia jata hai.