using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Common.Models;

public class BookBorrowingRequestDto
{
    public Guid Id { get; set; }
    [Required]
    public string RequesterName { get; set; } = null!;
    public string? ApproverName { get; set; }
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public DateTime DateRequested { get; set; }
    [Required]
    public DateTime ExpireDate { get; set; }
    [Required]
    public string RequestType { get; set; } = null!;
    public ICollection<BookBorrowingRequestDetailDto>? Details {get; set; }
}
