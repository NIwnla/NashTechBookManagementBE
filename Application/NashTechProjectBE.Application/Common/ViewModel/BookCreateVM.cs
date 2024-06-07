using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.Models.ViewModel;

public class BookCreateVM
{
    [Required]
    public Guid CreateUserId { get; set; }
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = null!;
    [Required]
    public string Body { get; set; } = null!;
    [Required]
    public int Quantity { get; set; }
    public ICollection<Guid>? CategoryIds { get; set; }
}
