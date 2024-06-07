using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class CategoryCreateUpdateVM
{
    [Required]
    public Guid CreateUpdateUserId { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
}
