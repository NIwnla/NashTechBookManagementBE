using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NashTechProjectBE.Domain.Enums;

namespace NashTechProjectBE.Domain.Entities;

public class Book
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = null!;
    [Required]
    public string Body { get; set; } = null!;
    [Required]
    public int Quantity { get; set; } = 0;
    public virtual BookBorrowingRequestDetail? BorrowedDetail { get; set; }
    public virtual ICollection<Category>? Categories { get; set; }

}
