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
                .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.Orders != null ? src.Orders.Count : 0))
                .ForMember(dest => dest.TotalSpent, opt => opt.MapFrom(src => src.Orders != null ? src.Orders.Sum(o => o.TotalPrice) : 0));

            // User Entity -> UserAuthDto (For login/register responses)
            CreateMap<User, UserAuthDto>();

            // UpdateUserDto -> User Entity (For profile updates)
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role ?? UserRole.Customer))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ?? true));
        }
    }
}
