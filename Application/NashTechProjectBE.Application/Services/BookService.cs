using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.Models.ViewModel;
using NashTechProjectBE.Domain.Contraints;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Services;

public class BookService : SortOrder, IBookService
{
    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<BookCategory> _bookCategoryRepository;
    private readonly IGenericRepository<BookBorrowingRequest> _requestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public BookService(IGenericRepository<Book> bookRepository,
                        IGenericRepository<Category> categoryRepository,
                        IGenericRepository<BookCategory> bookCategoryRepository,
                        IGenericRepository<BookBorrowingRequest> requestRepository,
                        IUserRepository userRepository,
                        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _bookCategoryRepository = bookCategoryRepository;
        _requestRepository = requestRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Book> BookExistAsync(Guid id)
    {
        return await _bookRepository.FindByCondition(b => b.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }


    public async Task<bool> BookNameExistAsync(string name)
    {
        return await _bookRepository.FindByCondition(b => b.Title == name).FirstOrDefaultAsync() != null;
    }


    public async Task<Result> CreateAsync(BookCreateVM bookForm)
    {
        if (await BookNameExistAsync(bookForm.Title))
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There' s already book with name: {bookForm.Title}" };
        }
        var book = _mapper.Map<Book>(bookForm);
        book.Categories = GetCategories(bookForm.CategoryIds);
        if (await _bookRepository.CreateAsync(book, bookForm.CreateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Book added, Id: {book.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when creating book, Id: {book.Id}" };
        }

    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var bookToDelete = await BookExistAsync(id);
        if (bookToDelete == null)
        {
            return new Result { StatusCode = HttpStatusCode.NotFound, Message = $"There's no book to delete with Id: {id} " };
        }
        if (await _bookRepository.DeleteAsync(bookToDelete))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Book deleted, Id: {bookToDelete.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when deleting book, Id: {bookToDelete.Id}" };
        }

    }

    public async Task<IEnumerable<AvailableBookDto>> GetAvailableBooksAsync(Guid requestId, Guid userId)
    {
        var currentRequest = await _requestRepository.FindByCondition(r => r.Id == requestId && r.UserId == userId)
                                                        .Include(r => r.Details)
                                                        .FirstOrDefaultAsync();
        var currentRequestDetails = currentRequest.Details.Select(d => d.BookId).ToList();
        var books = await _bookRepository
                                        .FindByCondition(b => b.Quantity > 0 && (currentRequestDetails.IsNullOrEmpty() || (!currentRequestDetails.IsNullOrEmpty() && !currentRequestDetails.Any(d => d == b.Id))))
                                        .ToListAsync();
        var bookDtos = _mapper.Map<IEnumerable<AvailableBookDto>>(books);
        return bookDtos;
    }

    public async Task<BookDto> GetBookAsync(Guid id)
    {
        var book = await _bookRepository.FindByCondition(b => b.Id == id).Include(b => b.Categories).FirstOrDefaultAsync();
        if (book == null)
        {
            return null;
        }
        var bookDto = _mapper.Map<BookDto>(book);
        return bookDto;
    }


    public async Task<PaginatedList<BookDto, Book>> GetBooksAsync(int pageNumber, int pageSize, string sort, string order, string search)
    {
        var query = _bookRepository.FindAll();
        // Search
        if (!search.IsNullOrEmpty())
        {
            query = query.Where(book => book.Title.ToLower().Contains(search.ToLower()) || book.Body.ToLower().Contains(search.ToLower()));
        }
        // Sort
        if (!sort.IsNullOrEmpty())
        {
            if (order.ToUpper() == SORT_ASCENDING)
            {
                switch (sort)
                {
                    case nameof(Book.Quantity):
                        query = query.OrderBy(book => book.Quantity);
                        break;
                    case nameof(Book.Body):
                        query = query.OrderBy(book => book.Body);
                        break;
                    case nameof(Book.Title):
                        query = query.OrderBy(book => book.Title);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case nameof(Book.Quantity):
                        query = query.OrderByDescending(book => book.Quantity);
                        break;
                    case nameof(Book.Body):
                        query = query.OrderByDescending(book => book.Body);
                        break;
                    case nameof(Book.Title):
                        query = query.OrderByDescending(book => book.Title);
                        break;
                }
            }
        }

        query = query.Include(b => b.Categories);

        // Pagination
        var paginatedBooks = await PaginatedList<BookDto, Book>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize, _mapper);
        return paginatedBooks;
    }

    public List<Category>? GetCategories(ICollection<Guid>? categoryIds)
    {
        if (categoryIds != null && categoryIds.Count > 0)
        {
            return _categoryRepository.FindByCondition(c => categoryIds.Contains(c.Id)).ToList();
        }
        return null;
    }


    public async Task<Result> UpdateAsync(Guid id, BookUpdateVM bookForm)
    {
        var bookToUpdate = await BookExistAsync(id);
        if (bookToUpdate == null)
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There's no book to delete with Id: {id} " };
        }
        var currentBookTitle = bookToUpdate.Title;
        if (!string.Equals(currentBookTitle, bookForm.Title, StringComparison.OrdinalIgnoreCase) && await BookNameExistAsync(bookForm.Title))
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There' s already book with name: {bookForm.Title}" };
        }
        var book = _mapper.Map<Book>(bookForm);
        book.Id = id;
        var bookCategories = _bookCategoryRepository.FindByCondition(bc => bc.BookId == book.Id).AsEnumerable();
        book.Categories = GetCategories(bookForm.CategoryIds);
        if (bookCategories.Count() > 0)
        {
            await _bookCategoryRepository.DeleteRangeAsync(bookCategories);
        }
        if (await _bookRepository.UpdateAsync(book, bookForm.UpdateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Book updated, Id: {book.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when updating book, Id: {book.Id}" };
        }

    }

}
