using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    // Controller for managing author CRUD operations and book relationships
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        // GET: api/Author - Retrieves all authors ordered by name
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            try
            {
                var query = _context.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        FullName = a.FullName,
                        Biography = a.Biography,
                        CreatedDate = a.CreatedDate,
                        BookCount = a.AuthorBooks.Count,
                        Books = new List<BookSummaryDto>() // Empty for list view performance
                    });
                return Ok(await query.ToListAsync());
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Author/{id} - Retrieves a specific author by ID with their books
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                // Single efficient query with includes and projection
                var authorDto = await _context.Authors
                    .Where(a => a.Id == id)
                    .Include(a => a.AuthorBooks)
                    .ThenInclude(ab => ab.Book)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        FullName = a.FullName,
                        Biography = a.Biography,
                        CreatedDate = a.CreatedDate,
                        BookCount = a.AuthorBooks.Count,
                        Books = a.AuthorBooks.Select(ab => new BookSummaryDto
                        {
                            Id = ab.Book.Id,
                            Title = ab.Book.Title,
                            Price = ab.Book.Price,
                            ImageUrl = ab.Book.ImageUrl,
                            GenreName = ab.Book.Genre != null ? ab.Book.Genre.Name : null,
                            InStock = ab.Book.Stock > 0
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (authorDto == null)
                {
                    return NotFound();
                }

                return Ok(authorDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Author/search?query=term - Search authors by first name, last name, or biography
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty.");
                }

                var lowerQuery = query.ToLower();

                var results = await _context.Authors
                     .Where(a => a.FirstName.ToLower().Contains(lowerQuery) ||
                                 a.LastName.ToLower().Contains(lowerQuery) ||
                                 (a.Biography != null && a.Biography.ToLower().Contains(lowerQuery)))
                     .OrderBy(a => a.LastName)
                     .Select(a => new AuthorDto
                     {
                         Id = a.Id,
                         FirstName = a.FirstName,
                         LastName = a.LastName,
                         FullName = a.FullName,
                         Biography = a.Biography,
                         CreatedDate = a.CreatedDate,
                         BookCount = a.AuthorBooks.Count,
                         Books = new List<BookSummaryDto>() // Empty for search results performance
                     })
                     .ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        // POST: api/Author - Creates a new author
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = new Author
                {
                    FirstName = createAuthorDto.FirstName,
                    LastName = createAuthorDto.LastName,
                    Biography = createAuthorDto.Biography,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                // Map to DTO for response
                var authorDto = new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    FullName = author.FullName,
                    Biography = author.Biography,
                    CreatedDate = author.CreatedDate,
                    BookCount = 0, // New author has no books yet
                    Books = new List<BookSummaryDto>()
                };

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, authorDto);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while creating author.");
            }
        }

        #endregion

        #region UPDATE Operations

        // PUT: api/Author/{id} - Updates an existing author
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                // Update properties
                author.FirstName = updateAuthorDto.FirstName;
                author.LastName = updateAuthorDto.LastName;
                author.Biography = updateAuthorDto.Biography;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while updating author.");
            }


        }

        #endregion

        #region DELETE Operations

        // DELETE: api/Author/{id} - Deletes an author (with business rule checks)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                // Find author with related AuthorBooks
                var author = await _context.Authors
                    .Include(a => a.AuthorBooks)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound();
                }

                // Check if author has any books
                if (author.AuthorBooks.Any())
                {
                    return BadRequest("Cannot delete author with existing books. Remove book associations first.");
                }

                // Safe to delete author
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while deleting author.");
            }

        }

        #endregion

        #region Helper Methods

        // Checks if an author exists in the database
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(a => a.Id == id);
        }

        // Maps Author entity to AuthorDto
        private AuthorDto MapToAuthorDto(Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                FullName = author.FullName,
                Biography = author.Biography,
                CreatedDate = author.CreatedDate,
                BookCount = author.AuthorBooks?.Count ?? 0,
                Books = author.AuthorBooks?.Select(ab => new BookSummaryDto
                {
                    Id = ab.Book.Id,
                    Title = ab.Book.Title,
                    Price = ab.Book.Price,
                    ImageUrl = ab.Book.ImageUrl,
                    GenreName = ab.Book.Genre?.Name,
                    InStock = ab.Book.Stock > 0
                }).ToList() ?? new List<BookSummaryDto>()
            };
        }

        #endregion
    }
}
