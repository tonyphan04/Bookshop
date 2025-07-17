using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace BookshopMVC.Controllers
{
    // Controller for managing genre CRUD operations
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GenreController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region READ Operations

        // GET: api/Genre - Retrieves all genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
        {
            try
            {
                // Use AutoMapper with ProjectTo for efficient database projection
                var genres = await _context.Genres
                    .OrderBy(g => g.Name)
                    .ProjectTo<GenreDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // For list view, we want empty books for performance
                foreach (var genre in genres)
                {
                    genre.Books = new List<BookSummaryDto>();
                }

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
                // Find genre by ID with related books included
                var genre = await _context.Genres
                    .Include(g => g.Books)
                    .ThenInclude(b => b.Genre)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                // Use AutoMapper to map to DTO
                var genreDto = _mapper.Map<GenreDto>(genre);

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

                // Search genres by name or description with AutoMapper projection
                var result = await _context.Genres
                    .Where(g => g.Name.ToLower().Contains(lowerQuery) ||
                               (g.Description != null && g.Description.ToLower().Contains(lowerQuery)))
                    .OrderBy(g => g.Name)
                    .ProjectTo<GenreDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // For search results, we want empty books for performance
                foreach (var genre in result)
                {
                    genre.Books = new List<BookSummaryDto>();
                }

                return Ok(result);
            }
            catch (Exception ex)
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

                // Check if genre with same name already exists using helper method
                var genreExists = await GenreNameExists(createGenreDto.Name);
                if (genreExists)
                {
                    return BadRequest("Genre already exists.");
                }

                // Use AutoMapper to create new Genre entity from DTO
                var genre = _mapper.Map<Genre>(createGenreDto);

                // Add to context and save
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();

                // Use AutoMapper to map to DTO for response
                var genreDto = _mapper.Map<GenreDto>(genre);

                // Return 201 Created with location header
                return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genreDto);
            }
            catch (Exception ex)
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

                // Find the genre to update
                var genre = await _context.Genres.FindAsync(id);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id} not found.");
                }

                // Check for name conflicts using helper method
                var nameExists = await GenreNameExists(updateGenreDto.Name, id);
                if (nameExists)
                {
                    return BadRequest("Genre name already exists.");
                }

                // Use AutoMapper to update properties from DTO
                _mapper.Map(updateGenreDto, genre);

                // Save changes to database
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
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
            // Query genres where name matches (case-insensitive)
            var query = _context.Genres
                .Where(g => g.Name.ToLower() == name.ToLower());

            // If excludeId is provided, exclude that genre from search (for updates)
            if (excludeId.HasValue)
            {
                query = query.Where(g => g.Id != excludeId.Value);
            }

            // Use AnyAsync to check existence
            return await query.AnyAsync();
        }

        #endregion
    }
}
