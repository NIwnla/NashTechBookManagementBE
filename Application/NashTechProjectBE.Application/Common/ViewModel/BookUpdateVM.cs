using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.Models.ViewModel;

public class BookUpdateVM
{
    [Required]
    public Guid UpdateUserId { get; set; }
    [Required]
    [StringLength(500)]
    public string Title { get; set; } = null!;
    [Required]
    public string Body { get; set; } = null!;
    [Required]
    public int Quantity { get; set; } = 0;
    public ICollection<Guid>? CategoryIds { get; set; }
}
