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
public class RequestDetailsController : ControllerBase
{
    private readonly IBookBorrowingRequestDetailService _detailService;
    public RequestDetailsController(IBookBorrowingRequestDetailService detailService)
    {
        _detailService = detailService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index(
        [PositiveInt] int pageNumber = 1,
        [PositiveInt] int pageSize = 20,
        Guid? userId = null,
        int? status = null,
        string? title = null)
    {
        var details = await _detailService.GetDetailsAsync(pageNumber, pageSize, userId, status, title);
        if (details == null || details.TotalCount <= 0)
        {
            return NotFound("No record found in details");
        }
        return Ok(details);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var detailDto = await _detailService.GetDetailAsync(id);
        if (detailDto == null)
        {
            return NotFound($"No detail with Id :{id} found");
        }
        return Ok(detailDto);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BookBorrowingRequestDetailCreateVM detailForm)
    {
        var result = await _detailService.CreateAsync(detailForm);
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

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] BookBorrowingRequestDetailUpdateVM detailForm, Guid id)
    {
        var result = await _detailService.UpdateAsync(id, detailForm);
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

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid id)
    {
        var result = await _detailService.DeleteAsync(id, userId);
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

    [Authorize(Roles = Roles.ROLE_SUPERUSER)]
    [HttpPut("return")]
    public async Task<IActionResult> Put([FromBody]BookBorrowingRequestDetailReturnVM returnForm)
    {
        var result = await _detailService.ReturnBookAsync(returnForm.Id, returnForm.UserId);
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
