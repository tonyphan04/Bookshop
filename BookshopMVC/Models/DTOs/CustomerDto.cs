using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Customer information DTO excluding sensitive data like passwords.
    /// Used for customer profiles, admin customer management, and order displays.
    /// Includes computed TotalOrders for customer statistics.
    /// </summary>
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// Total number of orders placed by this customer (computed field)
        /// </summary>
        public int TotalOrders { get; set; }
    }

    /// <summary>
    /// DTO for customer registration with password validation.
    /// Used in registration forms and customer sign-up API endpoints.
    /// Includes password confirmation and comprehensive validation rules.
    /// </summary>
    public class CreateCustomerDto
    {
        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
    }

    /// <summary>
    /// DTO for updating customer profile information.
    /// Used in customer profile edit forms and account management.
    /// Excludes password changes (handled separately for security).
    /// </summary>
    public class UpdateCustomerDto
    {
        [Required]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
    }

    /// <summary>
    /// DTO for customer login authentication.
    /// Used in login forms and authentication API endpoints.
    /// Contains only credentials needed for authentication.
    /// </summary>
    public class CustomerLoginDto
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    /// <summary>
    /// DTO for changing customer password with security validation.
    /// Used in password change forms and security settings.
    /// Requires current password verification for security.
    /// </summary>
    public class ChangePasswordDto
    {
        [Required]
        public string? CurrentPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmNewPassword { get; set; }
    }

    /// <summary>
    /// Minimal customer information DTO for references and dropdowns.
    /// Used when only basic customer identification is needed.
    /// Optimized for performance in list scenarios and quick lookups.
    /// </summary>
    public class CustomerSummaryDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
