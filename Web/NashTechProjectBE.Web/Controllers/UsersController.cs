using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Common.ViewModel;

namespace NashTechProjectBE.Web.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly AppSettings _applicationSettings;
		private readonly IUserService _userService;
		public UsersController(IOptions<AppSettings> applicationSettings, IUserService userService)
		{
			_applicationSettings = applicationSettings.Value;
			_userService = userService;
		}
		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel login)
		{
			var key = Encoding.ASCII.GetBytes(this._applicationSettings.Secret);
			var result = await _userService.LoginAsync(login, key);
			if (result.StatusCode == HttpStatusCode.BadRequest)
			{
				return BadRequest(result.Message);
			}
			if (result.StatusCode == HttpStatusCode.OK)
			{
				return Ok(result.Message);
			}
			ModelState.AddModelError("", "Error when logging in");
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel registerModel)
		{
			var result = await _userService.RegisterAsync(registerModel);
			if (result.StatusCode == HttpStatusCode.OK)
			{
				return Ok(result.Message);
			}
			if (result.StatusCode == HttpStatusCode.BadRequest)
			{
				return BadRequest(result.Message);
			}
			ModelState.AddModelError("", result.Message);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
