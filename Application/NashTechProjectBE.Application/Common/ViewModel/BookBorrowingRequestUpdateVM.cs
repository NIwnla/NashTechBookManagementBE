using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestUpdateVM
{
    [Required]
    public Guid UpdateUserId { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    public Guid? ApproverId { get; set; }
    [Required]
    public string Description { get; set; } = null!;
}
