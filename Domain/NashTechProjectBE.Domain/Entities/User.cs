using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NashTechProjectBE.Domain.Entities
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		[Required]
		[StringLength(50)]
		public string Username { get; set; } = null!;
		public byte[] PasswordSalt { get; set; } = null!;
		public byte[] PasswordHash { get; set; } = null!;
		[StringLength(100)]
		public string? Email { get; set; }
		[Required]
		[StringLength(10)]
		public string Role { get; set; } = null!;
		public int? RequestCount { get; set; }
		public DateTime? CountResetDate { get; set; }
		public virtual ICollection<Book>? BorrowedBooks {get; set; }
		public virtual ICollection<BookBorrowingRequest>? ApprovedRequests {get; set; }
		public virtual ICollection<BookBorrowingRequest>? BorrowingRequests {get; set; }
	}
}
