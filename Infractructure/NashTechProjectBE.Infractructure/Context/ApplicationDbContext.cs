using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Domain.Entities;
namespace NashTechProjectBE.Infractructure.Context
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<BookCategory> BookCategories { get; set; }
		public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
		public DbSet<BookBorrowingRequestDetail> BookBorrowingRequestDetails { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>(entity =>
			{
				entity.AddShadowProperties();
				entity
					.HasMany(user => user.BorrowingRequests)
					.WithOne(request => request.User)
					.HasForeignKey(request => request.UserId)
					.IsRequired(true);
			});

			builder.Entity<Book>(entity =>
			{
				entity.AddShadowProperties();
			});

			builder.Entity<Category>(entity =>
			{
				entity.AddShadowProperties();
				entity
					.HasMany(category => category.Books)
					.WithMany(book => book.Categories)
					.UsingEntity<BookCategory>();
			});

			builder.Entity<BookCategory>().HasKey(bc => new { bc.BookId , bc.CategoryId});

			builder.Entity<BookBorrowingRequest>(entity =>
			{
				entity.AddShadowProperties();
				entity
					.HasMany(request => request.Details)
					.WithOne(detail => detail.Request)
					.HasForeignKey(detail => detail.RequestId)
					.IsRequired(true);
				entity
					.HasOne(request => request.Approver)
					.WithMany(user => user.ApprovedRequests)
					.HasForeignKey(request => request.ApproverId)
					.IsRequired(false);
			});

			builder.Entity<BookBorrowingRequestDetail>(entity =>
			{
				entity.AddShadowProperties();


			});

			base.OnModelCreating(builder);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);


			foreach (var entityEntry in entries)
			{
				if (entityEntry.Properties.Any(e => e.Metadata.Name == "UpdatedTime"))
				{
					entityEntry.Property("UpdatedTime").CurrentValue = DateTime.Now;

					if (entityEntry.State == EntityState.Added)
					{
						entityEntry.Property("CreatedTime").CurrentValue = DateTime.Now;
					}
				}
			}
			return await base.SaveChangesAsync();
		}

		public async Task<int> SaveChangesAsync(Guid? userId)
		{
			if (userId != null)
			{
				var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
				foreach (var entityEntry in entries)
				{
					if (entityEntry.Properties.Any(e => e.Metadata.Name == "UpdatedBy"))
					{
						entityEntry.Property("UpdatedBy").CurrentValue = userId;
						if (entityEntry.State == EntityState.Added)
						{
							entityEntry.Property("CreatedBy").CurrentValue = userId;
						}
					}
				}
			}
			return await SaveChangesAsync();
		}
		public T GetShadowProperty<T>(string shadowPropertyName, object entity)
		{
			var entry = Entry(entity);
			T? value = default;
			if (entry != null)
			{
				if (entry.Properties.Any(e => e.Metadata.Name == shadowPropertyName))
				{
					value = (T?)entry.Property(shadowPropertyName).CurrentValue;
				}
			}
			return value;
		}

	}
}
