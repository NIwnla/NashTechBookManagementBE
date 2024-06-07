using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Entities;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestCreateVM
{
    [Required]
    public Guid CreateUserId { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public int BorrowDays { get; set; }


}
