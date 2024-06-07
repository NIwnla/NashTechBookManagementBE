using System.ComponentModel.DataAnnotations;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Application.Common.ViewModel;

public class BookBorrowingRequestDetailUpdateVM
{
    public Guid UpdateUserId{get; set;}
    [Required]
    public RequestStatus RequestStatus { get; set; }
}
