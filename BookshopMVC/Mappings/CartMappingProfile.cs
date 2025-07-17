using AutoMapper;
using BookshopMVC.Models;
using BookshopMVC.DTOs;

namespace BookshopMVC.Mappings
{
    /// <summary>
    /// AutoMapper profile for Cart-related mappings
    /// Handles cart items and shopping cart data mappings
    /// </summary>
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            // CartItem Entity -> CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : string.Empty))
                .ForMember(dest => dest.BookPrice, opt => opt.MapFrom(src => src.Book != null ? src.Book.Price : 0))
                .ForMember(dest => dest.BookImageUrl, opt => opt.MapFrom(src => src.Book != null ? src.Book.ImageUrl : string.Empty))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * (src.Book != null ? src.Book.Price : 0)))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Book != null && src.Book.Stock >= src.Quantity));

            // For creating cart items from AddToCartDto
            CreateMap<AddToCartDto, CartItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set from authentication context
                .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
