using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ecommerce_API_Project.Validation
{
    public class EmailValidator:ValidationAttribute
    {
        /// Precompiled email regex (basic and efficient)
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",  // Adjust if you want stricter RFC-compliant pattern
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Set of disallowed placeholder emails
        private static readonly HashSet<string> InvalidEmails = new(StringComparer.OrdinalIgnoreCase)
        {
            "user@gmail.com",
            "string",
            "example@example.com"
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("Email is required.");
            }

            var email = value.ToString()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("Email cannot be empty.");
            }

            var normalizedEmail = email.Trim();

            if (!EmailRegex.IsMatch(normalizedEmail))
            {
                return new ValidationResult("Email format is invalid.");
            }

            if (InvalidEmails.Contains(normalizedEmail.ToLower()))
            {
                return new ValidationResult("Email cannot be a placeholder like 'user@gmail.com' or 'string'.");
            }

            return ValidationResult.Success;
        }
    }
}
 
