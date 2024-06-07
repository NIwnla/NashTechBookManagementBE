using System.ComponentModel.DataAnnotations;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestApproveVM
{
    [Required]
    public Guid ApproverId { get; set; }
    [Required]
    public Guid RequestId { get; set; }
    [Required]
    public bool Approve {get; set; }
}
