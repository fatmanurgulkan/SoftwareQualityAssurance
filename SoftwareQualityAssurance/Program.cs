using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.Repositories;
using SoftwareQualityAssurance.Services;

[assembly: InternalsVisibleTo("SoftwareQualityAssurance.IntegrationTests")]
[assembly: InternalsVisibleTo("SoftwareQualityAssurance.E2ETests")]

namespace SoftwareQualityAssurance;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Database Configuration
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repository Registration
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Service Registration
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IPropertyService, PropertyService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Global Exception Handler (500 Internal Server Error)
        app.UseMiddleware<Middleware.GlobalExceptionHandler>();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
