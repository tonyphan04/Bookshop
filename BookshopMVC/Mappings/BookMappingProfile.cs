using AutoMapper;
using BookshopMVC.Models;
using BookshopMVC.DTOs;

namespace BookshopMVC.Mappings
{
    /// <summary>
    /// AutoMapper profile for Book-related mappings
    /// Defines how to convert between Book entities and various DTOs
    /// </summary>
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile()
        {
            // Book Entity -> BookDto (Full details)
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.Name : string.Empty))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
                    src.AuthorBooks != null ?
                    src.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList() : new List<string>()));

            // Book Entity -> BookSummaryDto (List view)
            CreateMap<Book, BookSummaryDto>()
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Stock > 0))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.Name : string.Empty))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src =>
                    src.AuthorBooks != null ?
                    src.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList() : new List<string>()));

            // CreateBookDto -> Book Entity (For creating new books)
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Genre, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorBooks, opt => opt.Ignore());

            // UpdateBookDto -> Book Entity (For updating existing books)
            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Genre, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorBooks, opt => opt.Ignore());
        }
    }
}
