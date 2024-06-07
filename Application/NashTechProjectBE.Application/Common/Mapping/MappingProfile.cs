using AutoMapper;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.Models.ViewModel;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;
namespace NashTechProjectBE.Application.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BookCreateVM, Book>();
        CreateMap<BookUpdateVM, Book>();
        CreateMap<Book, BookDto>();
        CreateMap<Book, AvailableBookDto>();

        CreateMap<CategoryCreateUpdateVM, Category>();
        CreateMap<Category, CategoryDto>();

        CreateMap<BookBorrowingRequestCreateVM, BookBorrowingRequest>()
                .ForMember(
                    dest => dest.UserId,
                    src => src.MapFrom(x => x.CreateUserId)
                );
        CreateMap<BookBorrowingRequestUpdateVM, BookBorrowingRequest>();
        CreateMap<BookBorrowingRequest, BookBorrowingRequestDto>()
                .ForMember(
                    dest => dest.RequesterName,
                    src => src.MapFrom(x => x.User.Username)
                )
                .ForMember(
                    dest => dest.ApproverName,
                    src => src.MapFrom(x => x.Approver.Username)
                )
                .ForMember(
                    dest => dest.RequestType,
                    src => src.MapFrom(x => x.RequestType.ToString())
                );


        CreateMap<BookBorrowingRequestDetailCreateVM, BookBorrowingRequestDetail>();
        CreateMap<BookBorrowingRequestDetailUpdateVM, BookBorrowingRequestDetail>();
        CreateMap<BookBorrowingRequestDetail, BookBorrowingRequestDetailDto>()
                .ForMember(
                     dest => dest.BookName,
                    src => src.MapFrom(x => x.Book.Title)
                )
                .ForMember(
                    dest => dest.RequestStatus,
                    src => src.MapFrom(x => x.RequestStatus.ToString())
                );
                






    }
}
