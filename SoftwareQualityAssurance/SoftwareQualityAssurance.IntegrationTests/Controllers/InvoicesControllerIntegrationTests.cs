using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SoftwareQualityAssurance.IntegrationTests.Controllers;

public class InvoicesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public InvoicesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturnBadRequest_WhenAmountIsZero()
    {
        // Arrange
        var customer = await CreateTestCustomer();
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = 0,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            Status = "Pending"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoices", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturnBadRequest_WhenAmountIsNegative()
    {
        // Arrange
        var customer = await CreateTestCustomer();
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = -100,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            Status = "Pending"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoices", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInvoice_ShouldCreateInvoice_WhenValidDataProvided()
    {
        // Arrange
        var customer = await CreateTestCustomer();
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = 5000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            Status = "Pending"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invoices", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoice = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.TotalAmount.Should().Be(5000.00m);
    }

    [Fact]
    public async Task GetInvoiceById_ShouldReturnInvoice_WhenInvoiceExists()
    {
        // Arrange
        var customer = await CreateTestCustomer();
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-GET-001",
            TotalAmount = 7500.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            Status = "Paid"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/invoices", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdInvoice = await createResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        createdInvoice.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/invoices/{createdInvoice!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var invoice = await response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.Id.Should().Be(createdInvoice.Id);
        invoice.SerialNumber.Should().Be("INV-GET-001");
        invoice.Status.Should().Be("Paid");
    }

    private async Task<CustomerDto> CreateTestCustomer()
    {
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "Customer",
            Email = $"test{Guid.NewGuid()}@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        return customer!;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _scope.Dispose();
        _client.Dispose();
    }
}

