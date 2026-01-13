using System.Runtime.InteropServices;
using AutoMapper;
using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;

namespace ELibrary.WebApp.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<CheckoutRequestDTO,Checkout>();
            CreateMap<Employee,EmployeeResponseDTO>();
            CreateMap<EmployeeRequestDTO, Employee>();
            CreateMap<ReaderRequestDTO,Reader>();
            CreateMap<Reader,ReaderResponseDTO>();
            CreateMap<ReaderUpdateRequestDTO, Reader>().
                ForMember(dest=> dest.Password, otp => otp.Ignore()).
                ForMember(dest => dest.Email,opt => opt.Ignore()).
                ForMember(dest => dest.Status,otp => otp.Ignore());
                
            CreateMap<Book, BookResponseDTO>().
                ForMember(
                dest => dest.AuthorName,
                otp => otp.MapFrom(src => src.Author != null ? src.Author.AuthorName : string.Empty)
                ).
                ForMember(
                dest => dest.CategoryName,
                otp => otp.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty)
                ).
                ForMember(
                dest => dest.PublisherName,
                otp => otp.MapFrom(src => src.Publisher != null ? src.Publisher.PublisherName : string.Empty)
                );
            CreateMap<BookRequestDTO, Book>()
    .ForSourceMember(src => src.ImageFile, opt => opt.DoNotValidate())  
    .ForMember(dest => dest.Author, opt => opt.Ignore())
    .ForMember(dest => dest.Category, opt => opt.Ignore())
    .ForMember(dest => dest.Publisher, opt => opt.Ignore())
    .ForMember(dest => dest.Checkouts, opt => opt.Ignore())
    .ForMember(dest => dest.Reservations, opt => opt.Ignore());

            CreateMap<Reservation, ReservationResponseDTO>()
                .ForMember(
              dest => dest.ReaderName,
                opt => opt.MapFrom(src => src.Reader != null ?
              src.Reader.FirstName + " " + src.Reader.LastName :
              "Unknown Reader")

                ).
                ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Book.Title));
           
        }

        
    }
}
