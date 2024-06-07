using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
