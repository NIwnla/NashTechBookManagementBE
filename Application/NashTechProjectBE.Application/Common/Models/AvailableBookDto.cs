using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.Models;

public class AvailableBookDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = null!;
}
