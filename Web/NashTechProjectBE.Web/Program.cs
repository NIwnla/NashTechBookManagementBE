using Microsoft.EntityFrameworkCore;
using NashTechProjectBE.Infractructure.Context;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NashTechProjectBE.Application.Common.Mapping;
using NashTechProjectBE.Web.Extensions;
using NashTechProjectBE.Web.BackgroundJob;

namespace NashTechProjectBE.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			ConfigurationManager configuration = builder.Configuration;
			// Add services to the container.
			builder.Services.AddAutoMapper(typeof(MappingProfile));
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey
				});
				options.OperationFilter<SecurityRequirementsOperationFilter>();
			});
			builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("ApplicationSettings"));
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"), b => b.MigrationsAssembly("NashTechProjectBE.Infractructure"));
				options.EnableSensitiveDataLogging();
			});
			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}
			).AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ApplicationSettings:Secret"])),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			builder.Services.AddRepositories();
			builder.Services.AddServices();
			builder.Services.AddHostedService<HourlyUpdateService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors(policy => policy.AllowAnyHeader()
						   .AllowAnyMethod()
						   .SetIsOriginAllowed(origin => true)
						   .AllowCredentials());
			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();


			app.SeedData();

			app.Run();


		}
	}
}
