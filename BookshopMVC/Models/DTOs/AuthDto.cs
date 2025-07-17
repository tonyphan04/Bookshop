using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    // DTO for user login
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    // DTO for user registration
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [StrongPassword]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        [MaxLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
        public string? Address { get; set; }
    }

    // Custom validation attribute for strong password
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string password || string.IsNullOrEmpty(password))
                return false;

            // Check for at least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                ErrorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            // Check for at least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                ErrorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }

            // Check for at least one digit
            if (!password.Any(char.IsDigit))
            {
                ErrorMessage = "Password must contain at least one number.";
                return false;
            }

            // Check for at least one special character
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                ErrorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }
    }

    // DTO for authentication response
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserAuthDto? User { get; set; }
        public string? Token { get; set; } // For JWT if implemented
    }
}
