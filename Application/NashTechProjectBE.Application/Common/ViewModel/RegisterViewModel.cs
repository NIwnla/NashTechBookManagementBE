using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = null!;
    public string? Role { get; set; }
    [Required]
    [StringLength(1000, MinimumLength = 8, ErrorMessage = "Password must have at least 8 characters")]
    public string Password { get; set; } = null!;
    [Required]
    [Compare("Password", ErrorMessage = "Confirm password dont match with password")]
    public string ConfirmPassword { get; set; } = null!;
}
