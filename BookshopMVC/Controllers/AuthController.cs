using Microsoft.AspNetCore.Mvc;
using BookshopMVC.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookshopMVC.Application.Interfaces;

namespace BookshopMVC.Controllers
{
    // Controller for user authentication and account management
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Authentication Operations

        // POST: api/Auth/register - Register a new user
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(registerDto, ct);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "EmailExists" => Problem(title: "Email already registered.", statusCode: 409),
                    _ => Problem(title: "Registration failed.", statusCode: 500)
                };
            }

            return Ok(new AuthResponseDto { Success = true, Message = "Registration successful. Welcome to our bookshop!", User = result.Data });
        }

        // POST: api/Auth/login - User login
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 423)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(loginDto, ct);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "InvalidCredentials" => Problem(title: "Invalid credentials.", statusCode: 401),
                    "InactiveUser" => Problem(title: "Account is deactivated.", statusCode: 423),
                    _ => Problem(title: "Login failed.", statusCode: 500)
                };
            }

            // Create claims and sign in
            var u = result.Data!;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, u.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(u.FullName) ? u.Email : u.FullName),
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Role, u.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync("Cookies", claimsPrincipal);

            return Ok(new AuthResponseDto { Success = true, Message = "Login successful. Welcome back!", User = result.Data });
        }

        // POST: api/Auth/logout - User logout
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(204)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            // Sign out the user
            await HttpContext.SignOutAsync("Cookies");

            return NoContent();
        }

        #endregion

        #region User Management Operations

        // GET: api/Auth/profile/{id} - Get user profile
        [HttpGet("profile/me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<ActionResult<UserDto>> GetProfile(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Problem(title: "Unauthorized.", statusCode: 401);
            }

            var result = await _authService.GetProfileAsync(userId, ct);
            if (!result.Success)
            {
                return result.ErrorCode == "NotFound"
                    ? Problem(title: "User not found.", statusCode: 404)
                    : Problem(title: "Internal server error.", statusCode: 500);
            }
            return Ok(result.Data);
        }

        // PUT: api/Auth/profile/{id} - Update user profile
        [HttpPut("profile/me")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUserDto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Problem(title: "Unauthorized.", statusCode: 401);
            }

            var result = await _authService.UpdateProfileAsync(userId, updateUserDto, ct);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "EmailExists" => Problem(title: "Email already in use by another user.", statusCode: 409),
                    "NotFound" => Problem(title: "User not found.", statusCode: 404),
                    _ => Problem(title: "Internal server error.", statusCode: 500)
                };
            }

            return NoContent();
        }

        // PUT: api/Auth/change-password/{id} - Change user password
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                return Problem(title: "Unauthorized.", statusCode: 401);
            }

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto, ct);
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "InvalidCredentials" => Problem(title: "Current password is incorrect.", statusCode: 400),
                    "NotFound" => Problem(title: "User not found.", statusCode: 404),
                    _ => Problem(title: "Internal server error.", statusCode: 500)
                };
            }

            return NoContent();
        }

        #endregion
    }
}
