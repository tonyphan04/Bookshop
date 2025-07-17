using AutoMapper;
using BookshopMVC.Models;
using BookshopMVC.DTOs;

namespace BookshopMVC.Mappings
{
    /// <summary>
    /// AutoMapper profile for Authentication-related mappings
    /// Handles User registration, login responses, and user data mappings
    /// </summary>
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // RegisterDto -> User Entity (For user registration)
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => UserRole.Customer))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Set separately for security
                .ForMember(dest => dest.Orders, opt => opt.Ignore());

            // User Entity -> UserDto (For API responses)
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.OrderCount, opt => opt.Ignore()) // Set separately if needed
                .ForMember(dest => dest.TotalSpent, opt => opt.Ignore()); // Set separately if needed

            // User Entity -> AuthResponseDto (For login/register responses)
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Authentication successful"))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Token, opt => opt.Ignore()); // Set separately with JWT logic
        }
    }
}
