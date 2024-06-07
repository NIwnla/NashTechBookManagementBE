using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    public CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    public async Task<Result> CreateAsync(CategoryCreateUpdateVM categoryForm)
    {
        if (await CategoryNameExistAsync(categoryForm.Name))
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There' s already category with name: {categoryForm.Name}" };
        }
        var category = _mapper.Map<Category>(categoryForm);
        if (await _categoryRepository.CreateAsync(category, categoryForm.CreateUpdateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Category added, Id: {category.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when creating category, Id: {category.Id}" };
        }
    }

    public async Task<PaginatedList<CategoryDto, Category>> GetCategoriesAsync(int pageNumber, int pageSize, string? name, bool paginate)
    {
        var query = _categoryRepository.FindAll();
        if(!paginate){
            pageSize = await query.CountAsync();
        }
        if (!name.IsNullOrEmpty())
        {
            query = query.Where(c => c.Name.Contains(name));
        }
        return await PaginatedList<CategoryDto, Category>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize, _mapper);
    }

    public async Task<Result> UpdateAsync(Guid id, CategoryCreateUpdateVM categoryForm)
    {
        if (await CategoryNameExistAsync(categoryForm.Name))
        {
            return new Result { StatusCode = HttpStatusCode.BadRequest, Message = $"There' s already category with name: {categoryForm.Name}" };
        }
        var category = _mapper.Map<Category>(categoryForm);
        category.Id = id;
        if (await _categoryRepository.UpdateAsync(category, categoryForm.CreateUpdateUserId))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Category updated, Id: {category.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when updating category, Id: {category.Id}" };
        }
    }

    public async Task<bool> CategoryNameExistAsync(string name)
    {
        return await _categoryRepository.FindByCondition(c => c.Name == name).FirstOrDefaultAsync() != null;
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var categoryToDelete = await CategoryExistAsync(id);
        if (categoryToDelete == null)
        {
            return new Result { StatusCode = HttpStatusCode.NotFound, Message = $"There's no category to delete with Id: {id} " };
        }
        if (await _categoryRepository.DeleteAsync(categoryToDelete))
        {
            return new Result { StatusCode = HttpStatusCode.OK, Message = $"Category deleted, Id: {categoryToDelete.Id}" };
        }
        else
        {
            return new Result { StatusCode = HttpStatusCode.InternalServerError, Message = $"Error when deleting category, Id: {categoryToDelete.Id}" };
        }
    }

    public async Task<Category> CategoryExistAsync(Guid id)
    {
        return await _categoryRepository.FindByCondition(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.FindByCondition(c => c.Id == id).FirstOrDefaultAsync();
        if (category == null)
        {
            return null;
        }
        var categoryDto = _mapper.Map<CategoryDto>(category);
        return categoryDto;

    }

}
