using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    /// <summary>
    /// Controller for managing author CRUD operations.
    /// Handles API endpoints for author management.
    /// </summary>
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

        /// <summary>
        /// GET: api/Author
        /// Retrieves all authors
        /// </summary>
        /// <returns>List of AuthorDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            // TODO: Write LINQ to:
            // 1. Query all authors from _context.Authors
            // 2. Order by LastName, then FirstName
            // 3. Map to AuthorDto (use Select)
            // 4. Return Ok(authors)
            // 5. Add try-catch with StatusCode(500) for errors
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

        /// <summary>
        /// GET: api/Author/5
        /// Retrieves a specific author by ID with their books
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>AuthorDto with book information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            // TODO: Write LINQ to:
            // 1. Find author by ID using FirstOrDefaultAsync
            // 2. Include related AuthorBooks and Books (use Include and ThenInclude)
            // 3. Check if author == null, return NotFound
            // 4. Map to AuthorDto with book titles
            // 5. Return Ok(authorDto)
            // 6. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// GET: api/Author/search?query=robert
        /// Search authors by first name, last name, or biography
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <returns>List of AuthorDto matching the search</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string query)
        {
            // TODO: Write LINQ to:
            // 1. Validate query is not null/empty, return BadRequest if invalid
            // 2. Convert query to lowercase for case-insensitive search
            // 3. Query authors where FirstName, LastName, or Biography contains search term
            // 4. Use ToLower().Contains() for case-insensitive matching
            // 5. Order results by LastName
            // 6. Map to AuthorDto
            // 7. Return Ok(results)
            // 8. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// POST: api/Author
        /// Creates a new author
        /// </summary>
        /// <param name="createAuthorDto">Author creation data</param>
        /// <returns>Created AuthorDto</returns>
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Create new Author entity from DTO
            // 3. Set CreatedDate = DateTime.UtcNow
            // 4. Add to _context.Authors
            // 5. SaveChangesAsync()
            // 6. Map created author to AuthorDto
            // 7. Return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, authorDto)
            // 8. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// PUT: api/Author/5
        /// Updates an existing author
        /// </summary>
        /// <param name="id">Author ID to update</param>
        /// <param name="updateAuthorDto">Updated author data</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Find author by ID using FindAsync
            // 3. Check if author == null, return NotFound
            // 4. Update author properties from DTO
            // 5. SaveChangesAsync()
            // 6. Return NoContent()
            // 7. Add try-catch with StatusCode(500) for errors
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

        /// <summary>
        /// DELETE: api/Author/5
        /// Deletes an author (with business rule checks)
        /// </summary>
        /// <param name="id">Author ID to delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            // TODO: Write logic to:
            // 1. Find author by ID, include AuthorBooks relationships
            // 2. Check if author == null, return NotFound
            // 3. Check if author has any books (AuthorBooks.Any())
            // 4. If has books, return BadRequest("Cannot delete author with existing books")
            // 5. Remove all AuthorBooks relationships first
            // 6. Remove the author
            // 7. SaveChangesAsync()
            // 8. Return NoContent()
            // 9. Add try-catch with StatusCode(500) for errors
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

        /// <summary>
        /// Checks if an author exists in the database
        /// </summary>
        /// <param name="id">Author ID</param>
        /// <returns>True if author exists</returns>
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(a => a.Id == id);
        }

        /// <summary>
        /// Maps Author entity to AuthorDto
        /// </summary>
        /// <param name="author">Author entity</param>
        /// <returns>AuthorDto</returns>
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
