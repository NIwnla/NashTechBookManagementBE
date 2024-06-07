using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IBookBorrowingRequestDetailService
{
    Task<PaginatedList<BookBorrowingRequestDetailDto, BookBorrowingRequestDetail>> GetDetailsAsync(int pageNumber, int pageSize, Guid? userId, int? status, string? title);
    Task<Result> CreateAsync(BookBorrowingRequestDetailCreateVM detailForm);
    Task<Result> UpdateAsync(Guid id, BookBorrowingRequestDetailUpdateVM detailForm);
    Task<Result> DeleteAsync(Guid id, Guid userId);
    Task<BookBorrowingRequestDetail> DetailExistAsync(Guid id);
    Task<BookBorrowingRequestDetailDto> GetDetailAsync(Guid id);
    Task<Result> ReturnBookAsync(Guid id, Guid userId);
    
}
