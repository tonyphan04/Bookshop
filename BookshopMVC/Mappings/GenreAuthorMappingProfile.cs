using AutoMapper;
using BookshopMVC.Models;
using BookshopMVC.DTOs;

namespace BookshopMVC.Mappings
{
    /// <summary>
    /// AutoMapper profile for Genre and Author-related mappings
    /// Handles entity to DTO conversions with proper navigation property handling
    /// </summary>
    public class GenreAuthorMappingProfile : Profile
    {
        public GenreAuthorMappingProfile()
        {
            // Genre Entity -> GenreDto
            CreateMap<Genre, GenreDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books.Count))
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));

            // Author Entity -> AuthorDto
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.AuthorBooks.Count))
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.AuthorBooks.Select(ab => ab.Book)));

            // Author Entity -> AuthorSummaryDto
            CreateMap<Author, AuthorSummaryDto>();

            // Author Entity -> BookAuthorDto
            CreateMap<Author, BookAuthorDto>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorOrder, opt => opt.Ignore()); // Set manually based on business logic

            // CreateGenreDto -> Genre Entity
            CreateMap<CreateGenreDto, Genre>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Books, opt => opt.Ignore());

            // UpdateGenreDto -> Genre Entity
            CreateMap<UpdateGenreDto, Genre>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Books, opt => opt.Ignore());

            // CreateAuthorDto -> Author Entity
            CreateMap<CreateAuthorDto, Author>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.AuthorBooks, opt => opt.Ignore());

            // UpdateAuthorDto -> Author Entity  
            CreateMap<UpdateAuthorDto, Author>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorBooks, opt => opt.Ignore());

            // Book Entity -> BookSummaryDto (for Genre and Author detail views)
            CreateMap<Book, BookSummaryDto>()
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Stock > 0));
        }
    }
}
