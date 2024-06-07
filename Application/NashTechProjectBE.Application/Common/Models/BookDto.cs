using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Common.Models;

public class BookDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = null!;
    [Required]
    public string Body { get; set; } = null!;
    [Required]
    public int Quantity { get; set; } = 0;

    public ICollection<CategoryDto>? Categories { get; set; }

}
