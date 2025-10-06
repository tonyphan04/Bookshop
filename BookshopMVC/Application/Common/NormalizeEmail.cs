namespace BookshopMVC.Application.Common
{
    public static class EmailNormalizer
    {
        public static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}