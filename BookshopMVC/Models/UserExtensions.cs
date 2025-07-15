namespace BookshopMVC.Models
{
    public static class UserExtensions
    {
        public static bool IsCustomer(this User user)
        {
            return user.Role == UserRole.Customer;
        }

        public static bool IsAdmin(this User user)
        {
            return user.Role == UserRole.Admin;
        }

        public static bool CanManageBooks(this User user)
        {
            return user.IsAdmin();
        }

        public static bool CanManageOrders(this User user)
        {
            return user.IsAdmin();
        }

        public static bool CanManageUsers(this User user)
        {
            return user.IsAdmin();
        }

        public static bool CanPlaceOrders(this User user)
        {
            return user.IsCustomer() && user.IsActive;
        }
    }
}
