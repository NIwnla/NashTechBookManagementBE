using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestDetailCreateVM
{
    [Required]
    public Guid CreateUserId { get; set; }
    [Required]
    public Guid RequestId { get; set; }
    [Required]
    public Guid BookId { get; set; }
}
