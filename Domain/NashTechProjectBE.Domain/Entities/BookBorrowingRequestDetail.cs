using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Domain.Entities;

public class BookBorrowingRequestDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    public Guid RequestId { get; set; }
    [Required]
    public Guid BookId { get; set; }
    [Required]
    public RequestStatus RequestStatus { get; set; } = RequestStatus.Requesting;
    public virtual Book? Book { get; set; }
    public virtual BookBorrowingRequest? Request { get; set; }
}
