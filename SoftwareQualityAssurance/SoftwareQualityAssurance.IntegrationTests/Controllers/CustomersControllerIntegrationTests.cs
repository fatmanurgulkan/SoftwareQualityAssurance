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

public class CustomersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    public CustomersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
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
    public async Task GetAllCustomers_ShouldReturnEmptyList_WhenNoCustomersExist()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
        customers.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateCustomer_ShouldCreateCustomer_WhenValidDataProvided()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Email.Should().Be(createDto.Email);
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "duplicate@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created); // First one should succeed

        // Act - Try to create another customer with same email
        var response = await _client.PostAsJsonAsync("/api/customers", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var errorContent = await response.Content.ReadAsStringAsync();
        errorContent.Should().Contain("already exists");
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnCustomer_WhenCustomerExists()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "getbyid@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        createdCustomer.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(createdCustomer.Id);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldUpdateCustomer_WhenValidDataProvided()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "update@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        createdCustomer.Should().NotBeNull();

        var updateDto = new UpdateCustomerDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com",
            IdentityNumber = "12345678901",
            Balance = 20000,
            PhoneNumber = "05559876543"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/customers/{createdCustomer!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        updatedCustomer!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteCustomer_ShouldDeleteCustomer_WhenCustomerExists()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "delete@example.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "05551234567"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        createdCustomer.Should().NotBeNull();

        // Act
        var response = await _client.DeleteAsync($"/api/customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify customer is soft deleted
        var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _scope.Dispose();
        _client.Dispose();
    }
}

