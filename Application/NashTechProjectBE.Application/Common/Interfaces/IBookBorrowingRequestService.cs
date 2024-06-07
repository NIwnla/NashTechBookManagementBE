using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IBookBorrowingRequestService
{
    Task<PaginatedList<BookBorrowingRequestDto, BookBorrowingRequest>> GetRequestsAsync(int pageNumber, int pageSize, Guid? userId, int? type, string? title);
    Task<Result> CreateAsync(BookBorrowingRequestCreateVM requestForm);
    Task<Result> UpdateAsync(Guid id, BookBorrowingRequestUpdateVM requestForm);
    Task<Result> ApproveRequest(Guid id, Guid userId, bool approve);
    Task<BookBorrowingRequest> RequestExistAsync(Guid id);
    Task<BookBorrowingRequestDto> GetRequestAsync(Guid id);
}
