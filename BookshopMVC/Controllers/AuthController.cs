using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;

namespace BookshopMVC.Controllers
{
    // Controller for user authentication and account management
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Authentication Operations

        // POST: api/Auth/register - Register a new user
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                // Step 1: Basic input validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = string.Join("; ", errors)
                    });
                }

                // Step 2: Check if email already exists (prevent duplicates)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());

                if (existingUser != null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email already registered. Please use a different email or try logging in."
                    });
                }

                // Step 3: Hash password and create user
                var passwordHash = HashPassword(registerDto.Password);

                // 🎉 AutoMapper magic - replaces manual user creation!
                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = passwordHash; // Set separately for security

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Step 4: Return success response (user created but not logged in)
                var userAuth = _mapper.Map<UserAuthDto>(user);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful. Welcome to our bookshop!",
                    User = userAuth
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                });
            }
        }

        // POST: api/Auth/login - User login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                // Step 1: Basic input validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = string.Join("; ", errors)
                    });
                }

                // Step 2: Check if email exists (find user)
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

                if (user == null)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password. Please check your credentials and try again."
                    });
                }

                // Step 3: Check if password matches
                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password. Please check your credentials and try again."
                    });
                }

                // Step 4: Check if account is active
                if (!user.IsActive)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is deactivated. Please contact support for assistance."
                    });
                }

                // Step 5: Create claims for authentication (assign role)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Step 6: Sign in the user (complete authentication)
                await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                // Return success response using AutoMapper
                var userAuth = _mapper.Map<UserAuthDto>(user);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful. Welcome back!",
                    User = userAuth
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                });
            }
        }

        // POST: api/Auth/logout - User logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync("Cookies");

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Logout successful."
            });
        }

        #endregion

        #region User Management Operations

        // GET: api/Auth/profile/{id} - Get user profile
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserDto>> GetProfile(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Orders)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // 🎉 AutoMapper magic - converts User to UserDto with calculated properties!
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Auth/profile/{id} - Update user profile
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Check email uniqueness (excluding current user)
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == updateUserDto.Email.ToLower() && u.Id != id);

                if (emailExists)
                {
                    return BadRequest("Email already in use by another user.");
                }

                // 🎉 AutoMapper magic - updates user properties from DTO!
                _mapper.Map(updateUserDto, user);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Auth/change-password/{id} - Change user password
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Verify current password
                if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest("Current password is incorrect.");
                }

                // Hash new password
                user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        // Hash password using BCrypt or simple SHA256 for MVP
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BookshopSalt"));
            return Convert.ToBase64String(hashedBytes);
        }

        // Verify password against hash
        private bool VerifyPassword(string password, string hash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hash;
        }

        // Check if user exists
        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        #endregion
    }
}
