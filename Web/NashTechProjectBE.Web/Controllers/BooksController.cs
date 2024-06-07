using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Models.ViewModel;
using NashTechProjectBE.Application.Common.Validation;
using NashTechProjectBE.Domain.Contraints;

namespace NashTechProjectBE.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }
    [HttpGet]
    public async Task<IActionResult> Index([PositiveInt] int page = 1,
                                        [PositiveInt] int pageSize = 20,
                                        string sort = "",
                                        string order = "",
                                        string search = "")
    {
        var books = await _bookService.GetBooksAsync(page, pageSize, sort, order, search);
        if (books.TotalCount <= 0 || books == null)
        {
            return NotFound("No books record found");
        }
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var bookDto = await _bookService.GetBookAsync(id);
        if (bookDto == null)
        {
            return NotFound($"No book with Id :{id} found");
        }
        return Ok(bookDto);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBooks(Guid requestId ,Guid userId)
    {
        var bookDtos = await _bookService.GetAvailableBooksAsync(requestId,userId);
        if (bookDtos == null)
        {
            return NotFound("No available books record found");
        }
        return Ok(bookDtos);
    }
    [Authorize(Roles = Roles.ROLE_SUPERUSER)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BookCreateVM bookForm)
    {
        var result = await _bookService.CreateAsync(bookForm);
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
    public async Task<IActionResult> Put([FromBody] BookUpdateVM bookForm, Guid id)
    {
        var result = await _bookService.UpdateAsync(id, bookForm);
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
        var result = await _bookService.DeleteAsync(id);
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
