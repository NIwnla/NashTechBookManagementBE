using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.Models;

public class CategoryDto
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
}
