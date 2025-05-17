using System.ComponentModel.DataAnnotations;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class LoginVeiwModel
    {
        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; } = string.Empty;
    }
}
