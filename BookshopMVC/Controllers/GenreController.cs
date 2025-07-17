using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace BookshopMVC.Controllers
{
    // Controller for managing genre CRUD operations
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        // GET: api/Genre - Retrieves all genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
        {
            try
            {
                // ✅ Query all genres and project to DTO in database query for efficiency
                var genres = await _context.Genres
                    .OrderBy(g => g.Name)  // Order alphabetically by name
                    .Select(g => new GenreDto
                    {
                        Id = g.Id,                    // ✅ FIXED: Include Id (was missing)
                        Name = g.Name,
                        Description = g.Description,
                        IsActive = g.IsActive,        // ✅ FIXED: Include IsActive (was missing)
                        DisplayOrder = g.DisplayOrder,
                        BookCount = g.Books.Count(),  // Count books in this genre
                        Books = new List<BookSummaryDto>() // Empty for list view (performance)
                    })
                    .ToListAsync(); // Execute query once

                return Ok(genres);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Genre/5 - Retrieves a specific genre by ID with its books
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetGenre(int id)
        {
            try
            {
                // ✅ Find genre by ID with related books included
                var genre = await _context.Genres
                    .Include(g => g.Books)  // Include related books for detailed view
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                // ✅ FIXED: Complete DTO mapping with all properties
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name,                    // ✅ FIXED: Was missing
                    Description = genre.Description,      // ✅ FIXED: Was missing  
                    IsActive = genre.IsActive,           // ✅ FIXED: Was missing
                    DisplayOrder = genre.DisplayOrder,   // ✅ FIXED: Was missing
                    BookCount = genre.Books?.Count ?? 0,                // ✅ Map books to summary DTOs for detailed view
                    Books = genre.Books?.Select(b => new BookSummaryDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Price = b.Price,
                        GenreName = genre.Name,
                        InStock = b.Stock > 0  // ✅ FIXED: Use correct property name 'Stock'
                    }).ToList() ?? new List<BookSummaryDto>()
                };

                return Ok(genreDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Genre/search?query=fiction - Search genres by name or description
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> SearchGenres([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty.");
                }

                var lowerQuery = query.ToLower();

                // ✅ Search genres by name or description (handle null descriptions)
                var result = await _context.Genres
                    .Where(g => g.Name.ToLower().Contains(lowerQuery) ||
                               (g.Description != null && g.Description.ToLower().Contains(lowerQuery))) // ✅ FIXED: Null check
                    .OrderBy(g => g.Name)
                    .Select(g => new GenreDto
                    {
                        Id = g.Id,                    // ✅ FIXED: Include Id (was missing)
                        Name = g.Name,                // ✅ FIXED: Include Name (was missing)
                        Description = g.Description,
                        IsActive = g.IsActive,        // ✅ FIXED: Include IsActive (was missing)
                        DisplayOrder = g.DisplayOrder, // ✅ FIXED: Include DisplayOrder (was missing)
                        BookCount = g.Books.Count(),
                        Books = new List<BookSummaryDto>() // Empty for search performance
                    })
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        // POST: api/Genre - Creates a new genre (Admin Only)
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ✅ Check if genre with same name already exists using helper method
                var genreExists = await GenreNameExists(createGenreDto.Name);
                if (genreExists)
                {
                    return BadRequest("Genre already exists.");
                }

                // ✅ Create new Genre entity from DTO
                var genre = new Genre
                {
                    Name = createGenreDto.Name,
                    Description = createGenreDto.Description,
                    IsActive = createGenreDto.IsActive,
                    DisplayOrder = createGenreDto.DisplayOrder
                    // Note: Genre model doesn't have CreatedDate in MVP schema
                };

                // Add to context and save
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();

                // ✅ Map to DTO for response
                var genreDto = MapToGenreDto(genre);

                // ✅ Return 201 Created with location header
                return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genreDto);
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE Operations

        // PUT: api/Genre/{id} - Updates an existing genre (Admin Only)
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto updateGenreDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ✅ Find the genre to update
                var genre = await _context.Genres.FindAsync(id);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                // ✅ FIXED: Check for name conflicts using helper method
                var nameExists = await GenreNameExists(updateGenreDto.Name, id);
                if (nameExists)
                {
                    return BadRequest("Genre name already exists.");
                }

                // ✅ FIXED: Update ALL properties from DTO (not just Name)
                genre.Name = updateGenreDto.Name;
                genre.Description = updateGenreDto.Description;
                genre.IsActive = updateGenreDto.IsActive;
                genre.DisplayOrder = updateGenreDto.DisplayOrder;

                // Save changes to database
                await _context.SaveChangesAsync();
                return NoContent(); // ✅ FIXED: Return proper IActionResult
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region DELETE Operations

        // DELETE: api/Genre/{id} - Deletes a genre (Admin Only)
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            try
            {
                // ✅ Find genre by ID, include Books to check business rules
                var genre = await _context.Genres
                    .Include(g => g.Books) // Include books to check if deletion is allowed
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                // ✅ FIXED: Business rule - Don't allow deletion if genre has books
                if (genre.Books != null && genre.Books.Any())
                {
                    return BadRequest("Cannot delete genre with existing books. Please remove all books first.");
                }

                // Safe to delete - no books depend on this genre
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        // Checks if a genre exists in the database
        private async Task<bool> GenreExists(int id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }

        // Checks if a genre name already exists (for create/update validation)
        private async Task<bool> GenreNameExists(string name, int? excludeId = null)
        {
            // ✅ Query genres where name matches (case-insensitive)
            var query = _context.Genres
                .Where(g => g.Name.ToLower() == name.ToLower());

            // ✅ If excludeId is provided, exclude that genre from search (for updates)
            if (excludeId.HasValue)
            {
                query = query.Where(g => g.Id != excludeId.Value);
            }

            // ✅ Use AnyAsync to check existence
            return await query.AnyAsync();
        }

        // Maps Genre entity to GenreDto
        private GenreDto MapToGenreDto(Genre genre)
        {
            // ✅ Create and return new GenreDto with all properties mapped
            return new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description,
                IsActive = genre.IsActive,
                DisplayOrder = genre.DisplayOrder,
                // ✅ Calculate book count from genre.Books (safe null handling)
                BookCount = genre.Books?.Count ?? 0,
                // ✅ Create book summaries if Books are loaded, empty list otherwise
                Books = genre.Books?.Select(b => new BookSummaryDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Price = b.Price,
                    GenreName = genre.Name,
                    InStock = b.Stock > 0
                }).ToList() ?? new List<BookSummaryDto>()
            };
        }

        #endregion
    }
}
