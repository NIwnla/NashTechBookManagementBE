using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<PaginatedList<CategoryDto, Category>> GetCategoriesAsync(int pageNumber, int pageSize, string? name, bool paginate);
    Task<CategoryDto> GetCategoryByIdAsync(Guid id);
    Task<Result> CreateAsync(CategoryCreateUpdateVM categoryForm);
    Task<Result> UpdateAsync(Guid id, CategoryCreateUpdateVM categoryForm);
    Task<Result> DeleteAsync(Guid id);
    Task<bool> CategoryNameExistAsync(string name);
    Task<Category> CategoryExistAsync(Guid id);

}
