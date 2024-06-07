using NashTechProjectBE.Application.Common.Interfaces;
using NashTechProjectBE.Application.Services;
using NashTechProjectBE.Domain.Entities;
using NashTechProjectBE.Infractructure.Repositories;

namespace NashTechProjectBE.Web.Extensions
{
    public static class ServiceExtensions
	{
		public static IServiceCollection AddRepositories(this IServiceCollection services){
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IGenericRepository<Book>, GenericRepository<Book>>();
			services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
			services.AddScoped<IGenericRepository<BookCategory>,GenericRepository<BookCategory>>();
			services.AddScoped<IGenericRepository<BookBorrowingRequest>,GenericRepository<BookBorrowingRequest>>();
			services.AddScoped<IGenericRepository<BookBorrowingRequestDetail>, GenericRepository<BookBorrowingRequestDetail>>();
			return services;
		}
		
		public static IServiceCollection AddServices(this IServiceCollection services){
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IBookService, BookService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IBookBorrowingRequestService, BookBorrowingRequestService>();
			services.AddScoped<IBookBorrowingRequestDetailService,BookBorrowingRequestDetailService>();
			return services;
		}
	}
}
