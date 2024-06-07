using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Domain.Enums;
using NashTechProjectBE.Infractructure.Context;

namespace NashTechProjectBE.Web.BackgroundJob
{
	public class HourlyUpdateService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<HourlyUpdateService> _logger;
		private Timer _timer;

		public HourlyUpdateService(IServiceProvider serviceProvider, ILogger<HourlyUpdateService> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
			return Task.CompletedTask;
		}

		private async void DoWork(object state)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

				try
				{
					var overdueRequestDetails = await dbContext.BookBorrowingRequestDetails.Include(d => d.Request)
																							.Where(d => d.Request.ExpireDate <= DateTime.Now && d.RequestStatus == RequestStatus.Borrowing)
																							.ToListAsync();
					var userRequestReset = await dbContext.Users.Where(u => u.CountResetDate <= DateTime.Now).ToListAsync();
					if(userRequestReset.Count > 0)
					{
						_logger.LogInformation("User request count resetable found:");
						foreach (var item in userRequestReset)
						{
							item.RequestCount = 3;
							item.CountResetDate = DateTime.Now.AddMonths(1);
							_logger.LogInformation(item.Id.ToString());
						}
						await dbContext.SaveChangesAsync();
						_logger.LogInformation("Users updated successfully.");
					}
					if (overdueRequestDetails.Count > 0)
					{
						_logger.LogInformation("Overdue request detail found:");
						foreach (var item in overdueRequestDetails)
						{
							item.RequestStatus = RequestStatus.Overdue;
							_logger.LogInformation(item.Id.ToString());
						}
						await dbContext.SaveChangesAsync();
						_logger.LogInformation("Details updated successfully.");
					}
					else
					{
						_logger.LogInformation("Task runned, no overdue request details found");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while updating the database.");
				}
			}
		}

		public override void Dispose()
		{
			_timer?.Dispose();
			base.Dispose();
		}
	}
}
