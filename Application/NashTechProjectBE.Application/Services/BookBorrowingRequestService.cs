using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Services;

public class BookBorrowingRequestService : IBookBorrowingRequestService
{
    private readonly IGenericRepository<BookBorrowingRequest> _requestRepository;
    private readonly IGenericRepository<BookBorrowingRequestDetail> _detailRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public BookBorrowingRequestService(IGenericRepository<BookBorrowingRequest> requestRepository,
                                        IGenericRepository<BookBorrowingRequestDetail> detailRepository,
                                        IUserRepository userRepository,
                                        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _detailRepository = detailRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<Result> CreateAsync(BookBorrowingRequestCreateVM requestForm)
    {
        var user = await _userRepository.GetByIdAsync(requestForm.CreateUserId);
        if (user.RequestCount <= 0)
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = "There already 3 request created this month" };
        }
        user.RequestCount = user.RequestCount - 1;
        var request = _mapper.Map<BookBorrowingRequest>(requestForm);
        request.DateRequested = DateTime.Now;
        request.ExpireDate = DateTime.Now.AddDays(requestForm.BorrowDays);
        if (await _requestRepository.CreateAsync(request, requestForm.CreateUserId) && await _userRepository.UpdateUseAsync(user))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request added, Id: {request.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when creating request, Id: {request.Id}" };
        }
    }

    public Task<PaginatedList<BookBorrowingRequestDto, BookBorrowingRequest>> GetRequestsAsync(
        int pageNumber,
        int pageSize,
        Guid? userId,
        int? type,
        string? title)
    {
        var query = _requestRepository.FindAll()
                                     .Include(r => r.User)
                                     .Include(r => r.Approver)
                                     .Include(r => r.Details).ThenInclude(d => d.Book)
                                     .Where(r => (userId == null || (userId != null && r.UserId == userId)) &&
                                                 (type == null || (type != null && (int)r.RequestType == type)) &&
                                                 (string.IsNullOrEmpty(title) || (!string.IsNullOrEmpty(title) && (r.Details.Any(d => d.Book.Title == title) || r.Title.Contains(title)))));
        return PaginatedList<BookBorrowingRequestDto, BookBorrowingRequest>
                .CreateAsync(query.AsNoTracking(), pageNumber, pageSize, _mapper);
    }

    public Task<BookBorrowingRequest> RequestExistAsync(Guid id)
    {
        return _requestRepository.FindByCondition(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Result> UpdateAsync(Guid id, BookBorrowingRequestUpdateVM requestForm)
    {
        var requestToUpdate = await RequestExistAsync(id);
        if (requestToUpdate == null)
        {
            return new Result { StatusCode = HttpStatusCode.NotFound, Message = $"No request found, Id: {id}" };
        }
        var request = _mapper.Map<BookBorrowingRequest>(requestForm);
        request.Id = requestToUpdate.Id;
        if (await _requestRepository.UpdateAsync(request, requestForm.UpdateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request updated, Id: {request.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when updating request, Id: {request.Id}" };
        }
    }

    public async Task<BookBorrowingRequestDto> GetRequestAsync(Guid id)
    {
        var request = await _requestRepository.FindByCondition(r => r.Id == id)
                                                .Include(r => r.Details)
                                                .Include(r => r.User)
                                                .Include(r => r.Approver)
                                                .FirstOrDefaultAsync();
        if (request == null)
        {
            return null;
        }
        var requestDto = _mapper.Map<BookBorrowingRequestDto>(request);
        return requestDto;
    }

    public async Task<Result> ApproveRequest(Guid id, Guid userId, bool approve)
    {
        var request = await _requestRepository.FindByCondition(r => r.Id == id).Include(r => r.Details).Include(r => r.User).FirstOrDefaultAsync();
        if (request == null)
        {
            return new Result { StatusCode = HttpStatusCode.NotFound, Message = $"No request found to approve, Id: {id}" };
        }
        request.ApproverId = userId;
        if (approve)
        {
            for (int i = 0; i < request.Details.Count(); i++)
            {
                request.Details.ElementAt(i).RequestStatus = RequestStatus.Borrowing;
            }
            request.RequestType = RequestType.Accepted;
        }
        else
        {
            for (int i = 0; i < request.Details.Count(); i++)
            {
                request.Details.ElementAt(i).RequestStatus = RequestStatus.Rejected;
            }
            request.User.RequestCount = request.User.RequestCount + 1;
            request.RequestType = RequestType.Rejected;
        }
        if (await _requestRepository.UpdateAsync(request, userId) && await _detailRepository.UpdateRangeAsync(request.Details, userId) && await _userRepository.UpdateUseAsync(request.User))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Request approved, Id: {request.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when approving request, Id: {request.Id}" };
        }
    }

}
