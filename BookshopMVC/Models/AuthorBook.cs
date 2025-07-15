namespace BookshopMVC.Models
{
    /// <summary>
    /// Junction table for the many-to-many relationship between Authors and Books
    /// </summary>
    public class AuthorBook
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        /// <summary>
        /// Order of this author in the book's author list (1st author, 2nd author, etc.)
        /// </summary>
        public int AuthorOrder { get; set; } = 1;
    }
}
