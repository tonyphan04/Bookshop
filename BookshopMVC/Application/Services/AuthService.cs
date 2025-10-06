using AutoMapper;
using BookshopMVC.Data;
using BookshopMVC.DTOs;
using BookshopMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context, IMapper mapper, ILogger<AuthService> logger, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task<OperationResult<UserAuthDto>> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            try
            {
                // Normalize email
                var normalized = EmailNormalizer.NormalizeEmail(dto.Email);

                var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalized, ct);
                if (exists)
                {
                    return OperationResult<UserAuthDto>.Fail("EmailExists", "Email already registered.");
                }

                var user = _mapper.Map<User>(dto);
                // Ensure stored email is normalized consistently (trim + lower)
                user.Email = EmailNormalizer.NormalizeEmail(user.Email);
                // Hash password using ASP.NET Core Identity hasher
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

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
                var normalized = EmailNormalizer.NormalizeEmail(dto.Email);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, ct);
                if (user is null)
                {
                    return OperationResult<UserAuthDto>.Fail("InvalidCredentials", "Invalid email or password.");
                }

                // First, verify with Identity hasher
                var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                if (verify == PasswordVerificationResult.Failed)
                {
                    // Try legacy SHA256 variants (salted and unsalted) for transition
                    if (user.PasswordHash == LegacyHashPassword_Salted(dto.Password) ||
                        user.PasswordHash == LegacyHashPassword_Unsalted(dto.Password))
                    {
                        // Upgrade to Identity hash
                        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                        await _context.SaveChangesAsync(ct);
                        verify = PasswordVerificationResult.Success;
                    }
                }
                else if (verify == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // Upgrade rehash
                    user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                    await _context.SaveChangesAsync(ct);
                }

                if (verify == PasswordVerificationResult.Failed)
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

                var normalized = EmailNormalizer.NormalizeEmail(dto.Email);
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Id != userId && u.Email.ToLower() == normalized, ct);
                if (emailExists)
                {
                    return OperationResult.Fail("EmailExists", "Email already in use by another user.");
                }

                _mapper.Map(dto, user);
                // Ensure updated email is normalized consistently
                user.Email = EmailNormalizer.NormalizeEmail(user.Email);
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

                // Verify current password using Identity hasher; if it fails, try legacy
                var pv = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
                var currentValid = pv != PasswordVerificationResult.Failed;
                if (!currentValid)
                {
                    currentValid = user.PasswordHash == LegacyHashPassword_Salted(dto.CurrentPassword) ||
                                   user.PasswordHash == LegacyHashPassword_Unsalted(dto.CurrentPassword);
                    if (currentValid)
                    {
                        // Upgrade existing hash before setting new one (optional)
                        user.PasswordHash = _passwordHasher.HashPassword(user, dto.CurrentPassword);
                    }
                }

                if (!currentValid)
                {
                    return OperationResult.Fail("InvalidCredentials", "Current password is incorrect.");
                }

                // Set new password hash using Identity hasher
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
                await _context.SaveChangesAsync(ct);
                return OperationResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChangePassword failed for {UserId}", userId);
                return OperationResult.Fail("Unexpected", "Failed to change password.");
            }
        }

        // Legacy SHA256 hashing support for transition (previously used in service and seeder)
        private static string LegacyHashPassword_Salted(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "BookshopSalt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private static string LegacyHashPassword_Unsalted(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
