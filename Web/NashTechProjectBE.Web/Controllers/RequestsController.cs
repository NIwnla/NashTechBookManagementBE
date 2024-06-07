using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.Validation;
using NashTechProjectBE.Application.Common.ViewModel;

namespace NashTechProjectBE.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IBookBorrowingRequestService _requestService;
    public RequestsController(IBookBorrowingRequestService requestService){
        _requestService = requestService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
       [PositiveInt] int pageNumber = 1,
       [PositiveInt] int pageSize = 20,
       Guid? userId = null,
       int? type = null,
       string? title = null)
    {
        var details = await _requestService.GetRequestsAsync(pageNumber, pageSize, userId, type, title);
        if (details == null || details.TotalCount <= 0)
        {
            return NotFound("No record found in requests");
        }
        return Ok(details);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var requestDto = await _requestService.GetRequestAsync(id);
        if (requestDto == null)
        {
            return NotFound($"No detail with Id :{id} found");
        }
        return Ok(requestDto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] BookBorrowingRequestCreateVM requestForm)
    {
        var result = await _requestService.CreateAsync(requestForm);
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromBody] BookBorrowingRequestUpdateVM requestForm, Guid id)
    {
        var result = await _requestService.UpdateAsync(id, requestForm);
        if (result.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Message);
        }
        ModelState.AddModelError("", result.Message);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpPut("{id:guid}/approval")]
    public async Task<IActionResult> Put([FromBody] BookBorrowingRequestApproveVM requestForm)
    {
        var result = await _requestService.ApproveRequest(requestForm.RequestId, requestForm.ApproverId, requestForm.Approve);
        if (result.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(result.Message);
        }
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(result.Message);
        }
        ModelState.AddModelError("", result.Message);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}
