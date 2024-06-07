using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Validation;
using NashTechProjectBE.Application.Common.ViewModel;
using NashTechProjectBE.Domain.Contraints;

namespace NashTechProjectBE.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([PositiveInt] int page = 1,
                                            [PositiveInt] int pageSize = 20,
                                            string? search = null,
                                            bool paginate = true)
    {
        var categories = await _categoryService.GetCategoriesAsync(page,pageSize,search, paginate);
        if (categories == null || categories.TotalCount <= 0)
        {
            return NotFound("No category found");
        }
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var categoryDto = await _categoryService.GetCategoryByIdAsync(id);
        if (categoryDto == null)
        {
            return NotFound($"No category with Id :{id} found");
        }
        return Ok(categoryDto);
    }

    [Authorize(Roles = Roles.ROLE_SUPERUSER)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CategoryCreateUpdateVM categoryForm){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _categoryService.CreateAsync(categoryForm);
        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Message);
        }
        ModelState.AddModelError("", result.Message);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize(Roles = Roles.ROLE_SUPERUSER)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] CategoryCreateUpdateVM categoryForm, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _categoryService.UpdateAsync(id, categoryForm);
        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Message);
        }
        ModelState.AddModelError("", result.Message);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [Authorize(Roles = Roles.ROLE_SUPERUSER)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _categoryService.DeleteAsync(id);
        if (result.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Message);
        }
        ModelState.AddModelError("", result.Message);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
