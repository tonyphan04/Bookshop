using AutoMapper;
using BookshopMVC.Data;
using BookshopMVC.DTOs;
using BookshopMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using BookshopMVC.Application.Common;
using BookshopMVC.Application.Interfaces;

namespace BookshopMVC.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IMapper mapper, ILogger<AuthService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationResult<UserAuthDto>> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            try
            {
                // Normalize email
                var email = dto.Email.Trim();

                var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower(), ct);
                if (exists)
                {
                    return OperationResult<UserAuthDto>.Fail("EmailExists", "Email already registered.");
                }

                var user = _mapper.Map<User>(dto);
                user.PasswordHash = HashPassword(dto.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);

                var userAuth = _mapper.Map<UserAuthDto>(user);
                return OperationResult<UserAuthDto>.Ok(userAuth, "Registration successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                return OperationResult<UserAuthDto>.Fail("Unexpected", "Registration failed.");
            }
        }

        public async Task<OperationResult<UserAuthDto>> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            try
            {
                var email = dto.Email.Trim();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
                if (user is null || !VerifyPassword(dto.Password, user.PasswordHash))
                {
                    return OperationResult<UserAuthDto>.Fail("InvalidCredentials", "Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    return OperationResult<UserAuthDto>.Fail("InactiveUser", "Account is deactivated.");
                }

                var userAuth = _mapper.Map<UserAuthDto>(user);
                return OperationResult<UserAuthDto>.Ok(userAuth, "Login successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", dto.Email);
                return OperationResult<UserAuthDto>.Fail("Unexpected", "Login failed.");
            }
        }

        public Task<OperationResult> LogoutAsync(CancellationToken ct)
        {
            // Session/cookie sign-out is handled in controller via HttpContext
            return Task.FromResult(OperationResult.Ok("Logout successful."));
        }

        public async Task<OperationResult<UserDto>> GetProfileAsync(int userId, CancellationToken ct)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Orders)
                    .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user == null)
                {
                    return OperationResult<UserDto>.Fail("NotFound", "User not found.");
                }

                var dto = _mapper.Map<UserDto>(user);
                return OperationResult<UserDto>.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProfile failed for {UserId}", userId);
                return OperationResult<UserDto>.Fail("Unexpected", "Failed to retrieve profile.");
            }
        }

        public async Task<OperationResult> UpdateProfileAsync(int userId, UpdateUserDto dto, CancellationToken ct)
        {
            try
            {
                var user = await _context.Users.FindAsync(new object?[] { userId }, ct);
                if (user == null)
                {
                    return OperationResult.Fail("NotFound", "User not found.");
                }

                var emailExists = await _context.Users
                    .AnyAsync(u => u.Id != userId && u.Email.ToLower() == dto.Email.ToLower(), ct);
                if (emailExists)
                {
                    return OperationResult.Fail("EmailExists", "Email already in use by another user.");
                }

                _mapper.Map(dto, user);
                await _context.SaveChangesAsync(ct);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProfile failed for {UserId}", userId);
                return OperationResult.Fail("Unexpected", "Failed to update profile.");
            }
        }

        public async Task<OperationResult> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct)
        {
            try
            {
                var user = await _context.Users.FindAsync(new object?[] { userId }, ct);
                if (user == null)
                {
                    return OperationResult.Fail("NotFound", "User not found.");
                }

                if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                {
                    return OperationResult.Fail("InvalidCredentials", "Current password is incorrect.");
                }

                user.PasswordHash = HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync(ct);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePassword failed for {UserId}", userId);
                return OperationResult.Fail("Unexpected", "Failed to change password.");
            }
        }

        // TEMP: reuse current hashing until Phase 2 introduces IPasswordHasher
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BookshopSalt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashed = HashPassword(password);
            return hashed == hash;
        }
    }
}
