using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

// *****API HAS BEEN CREATED AND RUN SMOOTHLY********//
// CUSTOM DATA ANNOTATION CREATED BY USING VALIDATION ATTRIBUTE CLASS AND INHERIT

namespace Ecommerce_API_Project.Validation;
    public class PasswordValidator : ValidationAttribute // use for validation
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Password is Required");
            }

            var password = value as string;

            var hasNumber = new Regex(@"[0-9]+"); // regex use for customized pattern
            var hasUpperCase = new Regex(@"[A-Z]");
            var hasLowerCase = new Regex(@"[a-z]");
            var hasMinMaxCharacter = new Regex(@".{8,}");
            var hasSpecialchracter = new Regex(@"[\W]+");

            // Password Logic 

            if (!hasNumber.IsMatch(password)) //(!) = is not equal
            {
                return new ValidationResult("password must be atleat one numeric digit");
            }

            else if (!hasUpperCase.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one uppercase character");
            }

            else if (!hasLowerCase.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one lowercase character");
            }

            else if (!hasMinMaxCharacter.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast 8 character");
            }

            else if (!hasSpecialchracter.IsMatch(password))
            {
                return new ValidationResult("Password must be atleast one specialcharacter");
            }
            return ValidationResult.Success;
        }
    }

// *****API HAS BEEN CREATED AND RUN SMOOTHLY********//
