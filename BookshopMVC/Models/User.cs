using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.Models
{
    public enum UserRole
    {
        Customer = 1,
        Admin = 2
    }

    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string FullName => $"{FirstName} {LastName}";

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Simple address for MVP
        [MaxLength(300)]
        public string? Address { get; set; }

        // Account information
        public UserRole Role { get; set; } = UserRole.Customer;
        
        public bool IsActive { get; set; } = true;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Helper properties for easy role checking
        public bool IsCustomer => Role == UserRole.Customer;
        public bool IsAdmin => Role == UserRole.Admin;

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
