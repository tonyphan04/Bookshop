using System.ComponentModel.DataAnnotations;
using BookshopMVC.Models;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete user information DTO for user profile and admin management.
    /// Used for user detail pages, profile editing, and admin user management.
    /// Excludes sensitive information like password hash.
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        
        // Computed properties
        public bool IsCustomer { get; set; }
        public bool IsAdmin { get; set; }
        
        // Order statistics for admin view
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    /// <summary>
    /// DTO for creating new users with validation.
    /// Used in user registration and admin user creation.
    /// Includes password field for initial setup.
    /// </summary>
    public class CreateUserDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        public UserRole Role { get; set; } = UserRole.Customer;
    }

    /// <summary>
    /// DTO for updating existing users.
    /// Used in profile editing and admin user management.
    /// Excludes password and sensitive fields.
    /// </summary>
    public class UpdateUserDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        // Only admins can change roles and active status
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Lightweight user summary DTO for lists and dropdowns.
    /// Used in order management, user lists, and admin dashboards.
    /// Minimal information for performance in list scenarios.
    /// </summary>
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

    /// <summary>
    /// DTO for user authentication and login responses.
    /// Used in login/authentication processes.
    /// Contains minimal user info needed for session management.
    /// </summary>
    public class UserAuthDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for password change operations.
    /// Used in user profile password updates.
    /// </summary>
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
