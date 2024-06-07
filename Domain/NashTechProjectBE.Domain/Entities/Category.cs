using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NashTechProjectBE.Domain.Entities;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    public ICollection<Book>? Books{ get; set; }
}
