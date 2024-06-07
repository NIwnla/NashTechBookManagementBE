using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Services;

public class BookBorrowingRequestDetailService : IBookBorrowingRequestDetailService
{
    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<BookBorrowingRequestDetail> _detailRepository;
    private readonly IMapper _mapper;
    public BookBorrowingRequestDetailService(IGenericRepository<BookBorrowingRequestDetail> detailRepository,
                                            IGenericRepository<Book> bookRepository,
                                            IMapper mapper)
    {
        _bookRepository = bookRepository;
        _detailRepository = detailRepository;
        _mapper = mapper;
    }
    public async Task<Result> CreateAsync(BookBorrowingRequestDetailCreateVM detailForm)
    {
        var detailCount = await _detailRepository.FindByCondition(d => d.RequestId == detailForm.RequestId).CountAsync();
        if(detailCount >= 5)
        {
			return new Result { StatusCode = HttpStatusCode.BadRequest, Message = "This request have 5 books already" };
		}
        var book = await _bookRepository.FindByCondition(b => b.Id == detailForm.BookId).FirstOrDefaultAsync();
        if (book == null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"No book found, Id: {detailForm.BookId}" };
        }
        if (book.Quantity <= 0)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There is no book with title: {book.Title} available" };
        }
        book.Quantity = book.Quantity - 1;
        var detail = _mapper.Map<BookBorrowingRequestDetail>(detailForm);
        if (await _detailRepository.CreateAsync(detail, detailForm.CreateUserId) && await _bookRepository.UpdateAsync(book, detailForm.CreateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request detail added, Id: {detail.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when adding request detail, Id: {detail.Id}" };
        }
    }

    public async Task<Result> DeleteAsync(Guid id, Guid userId)
    {
        var detailToDelete = await DetailExistAsync(id);
        if (detailToDelete == null)
        {
            return new Result { StatusCode = HttpStatusCode.NotFound, Message = $"Not found request detail with Id: {id}" };
        }
        if (detailToDelete.RequestStatus != RequestStatus.Requesting)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"You can only delete request detail with requesting status" };
        }
        var book = await _bookRepository.FindByCondition(b => b.Id == detailToDelete.BookId).FirstOrDefaultAsync();
        book.Quantity = book.Quantity + 1;
        if (await _detailRepository.DeleteAsync(detailToDelete) && await _bookRepository.UpdateAsync(book, userId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request detail deleted, Id: {detailToDelete.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when deleting request detail, Id: {detailToDelete.Id}" };
        }
    }

    public async Task<BookBorrowingRequestDetail> DetailExistAsync(Guid id)
    {
        return await _detailRepository.FindByCondition(d => d.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<BookBorrowingRequestDetailDto> GetDetailAsync(Guid id)
    {
        var detail = await _detailRepository.FindByCondition(d => d.Id == id).Include(d => d.Book).FirstOrDefaultAsync();
        if (detail == null)
        {
            return null;
        }
        var detailDto = _mapper.Map<BookBorrowingRequestDetailDto>(detail);
        return detailDto;
    }

    public Task<PaginatedList<BookBorrowingRequestDetailDto, BookBorrowingRequestDetail>> GetDetailsAsync(
        int pageNumber,
        int pageSize,
        Guid? userId,
        int? status,
        string? title)
    {
        var query = _detailRepository.FindAll()
                                    .Include(d => d.Book)
                                    .Include(d => d.Request)
                                    .Where(d => (userId == null || (userId != null && d.Request.UserId == userId)) &&
                                                (status == null || (status != null && (int)d.RequestStatus == status)) &&
                                                (string.IsNullOrEmpty(title) || (!string.IsNullOrEmpty(title) && d.Book.Title.ToLower().Contains(title.ToLower()))));
        return PaginatedList<BookBorrowingRequestDetailDto, BookBorrowingRequestDetail>
                .CreateAsync(query.AsNoTracking(), pageNumber, pageSize, _mapper);
    }

    public async Task<Result> UpdateAsync(Guid id, BookBorrowingRequestDetailUpdateVM detailForm)
    {
        var detailToUpdate = await DetailExistAsync(id);
        if (detailToUpdate == null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"No detail found, Id: {id}" };
        }
        var detail = _mapper.Map<BookBorrowingRequestDetail>(detailForm);
        detail.Id = detailToUpdate.Id;
        if (await _detailRepository.UpdateAsync(detail, detailForm.UpdateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request detail updated, Id: {detail.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when updating request detail, Id: {detail.Id}" };
        }
    }

    public async Task<Result> ReturnBookAsync(Guid id, Guid userId){
        var detailToUpdate = await _detailRepository.FindByCondition(d => d.Id == id).FirstOrDefaultAsync();
        if (detailToUpdate == null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"No detail found, Id: {id}" };
        }
        detailToUpdate.RequestStatus = RequestStatus.Returned;
        var bookReturned = await _bookRepository.FindByCondition(b => b.Id == detailToUpdate.BookId).FirstOrDefaultAsync();
        bookReturned.Quantity = bookReturned.Quantity +1;
        if (await _detailRepository.UpdateAsync(detailToUpdate, userId) && await _bookRepository.UpdateAsync(bookReturned, userId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request detail updated, Id: {detailToUpdate.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when updating request detail, Id: {detailToUpdate.Id}" };
        }
    }
}
