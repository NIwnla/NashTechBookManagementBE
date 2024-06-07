using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Domain.Entities;

public class BookBorrowingRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public Guid? ApproverId { get; set; }
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
    public RequestType RequestType { get; set; } = RequestType.Waiting;
    public virtual User? User { get; set; }
    public virtual User? Approver { get; set; }
    public virtual IEnumerable<BookBorrowingRequestDetail>? Details { get; set; }
}
