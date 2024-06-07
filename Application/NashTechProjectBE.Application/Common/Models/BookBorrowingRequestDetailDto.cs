using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Common.Models;

public class BookBorrowingRequestDetailDto
{
    public Guid Id { get; set; }
    [Required]
    public string BookName { get; set; } = null!;
    [Required]
    public string RequestStatus { get; set; } = null!;
    
}
