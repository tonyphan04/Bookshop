using BookshopMVC.Application.Common;
using BookshopMVC.DTOs;

namespace BookshopMVC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<OperationResult<UserAuthDto>> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<OperationResult<UserAuthDto>> LoginAsync(LoginDto dto, CancellationToken ct);
        Task<OperationResult> LogoutAsync(CancellationToken ct);
        Task<OperationResult<UserDto>> GetProfileAsync(int userId, CancellationToken ct);
        Task<OperationResult> UpdateProfileAsync(int userId, UpdateUserDto dto, CancellationToken ct);
        Task<OperationResult> ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken ct);
    }
}
