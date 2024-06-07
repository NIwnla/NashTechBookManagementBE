using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.Models.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface IBookService
{
    List<Category>? GetCategories(ICollection<Guid>? categoryIds);
    Task<BookDto> GetBookAsync(Guid id);
    Task<IEnumerable<AvailableBookDto>> GetAvailableBooksAsync(Guid requestId, Guid userId);
    Task<PaginatedList<BookDto,Book>> GetBooksAsync(int pageNumber, int pageSize, string sort, string order, string search);
    Task<Result> CreateAsync(BookCreateVM bookForm);
    Task<Result> UpdateAsync(Guid id, BookUpdateVM bookForm);
    Task<Result> DeleteAsync(Guid id);

    Task<bool> BookNameExistAsync(string name);
    Task<Book> BookExistAsync(Guid id);
    
}
