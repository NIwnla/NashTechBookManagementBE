using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestDetailReturnVM
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
}
