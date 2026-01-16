using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SoftwareQualityAssurance.E2ETests.Scenarios;

public class CustomerE2ETests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ApplicationDbContext _context;

    private static readonly string _databaseName = "E2ETestDb_" + Guid.NewGuid().ToString();

    public CustomerE2ETests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase(_databaseName);
                    options.EnableSensitiveDataLogging();
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
    public async Task CompleteCustomerLifecycle_ShouldWorkEndToEnd()
    {
        // Scenario: Create a customer, update it, retrieve it, and then delete it

        // Step 1: Create Customer
        var createDto = new CreateCustomerDto
        {
            FirstName = "E2E",
            LastName = "Test",
            Email = $"e2e{Guid.NewGuid()}@test.com",
            IdentityNumber = "12345678901",
            Balance = 50000,
            PhoneNumber = "05551234567"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseContent = await createResponse.Content.ReadAsStringAsync();
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
        createdCustomer.Should().NotBeNull($"Response content: {responseContent}");
        createdCustomer!.Id.Should().BeGreaterThan(0, $"Customer ID should be greater than 0. Response: {responseContent}");

        // Step 2: Retrieve Created Customer
        var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
        var getResponseContent = await getResponse.Content.ReadAsStringAsync();
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK, $"GET request failed. Status: {getResponse.StatusCode}, Content: {getResponseContent}, Customer ID: {createdCustomer.Id}");
        var retrievedCustomer = await getResponse.Content.ReadFromJsonAsync<CustomerDto>();
        retrievedCustomer!.Email.Should().Be(createDto.Email);

        // Step 3: Update Customer
        var updateDto = new UpdateCustomerDto
        {
            FirstName = "Updated",
            LastName = "Customer",
            Email = $"updated{Guid.NewGuid()}@test.com",
            IdentityNumber = "12345678901",
            Balance = 75000,
            PhoneNumber = "05559876543"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/customers/{createdCustomer.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCustomer = await updateResponse.Content.ReadFromJsonAsync<CustomerDto>();
        updatedCustomer!.FirstName.Should().Be("Updated");

        // Step 4: Verify Update
        var verifyResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
        var verifiedCustomer = await verifyResponse.Content.ReadFromJsonAsync<CustomerDto>();
        verifiedCustomer!.Balance.Should().Be(75000);

        // Step 5: Delete Customer (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/customers/{createdCustomer.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 6: Verify Customer is Deleted
        var deletedGetResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
        deletedGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CustomerWithInvoiceFlow_ShouldWorkEndToEnd()
    {
        // Scenario: Create a customer, create an invoice for that customer, and verify the relationship

        // Step 1: Create Customer
        var customerDto = new CreateCustomerDto
        {
            FirstName = "Invoice",
            LastName = "Customer",
            Email = $"invoice{Guid.NewGuid()}@test.com",
            IdentityNumber = "12345678901",
            Balance = 100000,
            PhoneNumber = "05551234567"
        };

        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        // Step 2: Create Invoice for Customer
        var invoiceDto = new CreateInvoiceDto
        {
            SerialNumber = $"INV-{Guid.NewGuid()}",
            TotalAmount = 10000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer!.Id,
            Status = "Pending"
        };

        var invoiceResponse = await _client.PostAsJsonAsync("/api/invoices", invoiceDto);
        invoiceResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoice = await invoiceResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.CustomerId.Should().Be(customer.Id);

        // Step 3: Retrieve Invoice
        var getInvoiceResponse = await _client.GetAsync($"/api/invoices/{invoice.Id}");
        getInvoiceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedInvoice = await getInvoiceResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        retrievedInvoice!.TotalAmount.Should().Be(10000.00m);
    }

    [Fact]
    public async Task PropertyWithCategoryAndLocation_ShouldWorkEndToEnd()
    {
        // Scenario: Create category, location, and then a property that uses both

        // Step 1: Create Category
        var categoryDto = new CreateCategoryDto
        {
            Name = "E2E Category",
            Description = "Test Category"
        };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", categoryDto);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();

        // Step 2: Create Location
        var locationDto = new CreateLocationDto
        {
            CityName = "E2E City",
            PlateCode = "99"
        };
        var locationResponse = await _client.PostAsJsonAsync("/api/locations", locationDto);
        var location = await locationResponse.Content.ReadFromJsonAsync<LocationDto>();

        // Step 3: Create Property
        var propertyDto = new CreatePropertyDto
        {
            Title = "E2E Test Property",
            BlockNumber = "E2E-1",
            ParcelNumber = "E2E-P1",
            SquareMeters = 150.00m,
            Price = 500000.00m,
            CategoryId = category!.Id,
            LocationId = location!.Id,
            IsAvailable = true
        };

        var propertyResponse = await _client.PostAsJsonAsync("/api/properties", propertyDto);
        propertyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var property = await propertyResponse.Content.ReadFromJsonAsync<PropertyDto>();
        property.Should().NotBeNull();
        property!.CategoryId.Should().Be(category.Id);
        property.LocationId.Should().Be(location.Id);
    }

    [Fact]
    public async Task MultipleInvoicesForCustomer_ShouldWorkEndToEnd()
    {
        // Scenario: Create a customer and multiple invoices, then verify all invoices are linked correctly

        // Step 1: Create Customer
        var customerDto = new CreateCustomerDto
        {
            FirstName = "Multi",
            LastName = "Invoice",
            Email = $"multi{Guid.NewGuid()}@test.com",
            IdentityNumber = "12345678901",
            Balance = 200000,
            PhoneNumber = "05551234567"
        };

        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        customerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();

        // Step 2: Create First Invoice
        var invoice1Dto = new CreateInvoiceDto
        {
            SerialNumber = $"INV-1-{Guid.NewGuid()}",
            TotalAmount = 15000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer!.Id,
            Status = "Pending"
        };

        var invoice1Response = await _client.PostAsJsonAsync("/api/invoices", invoice1Dto);
        invoice1Response.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoice1 = await invoice1Response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice1.Should().NotBeNull();

        // Step 3: Create Second Invoice
        var invoice2Dto = new CreateInvoiceDto
        {
            SerialNumber = $"INV-2-{Guid.NewGuid()}",
            TotalAmount = 25000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = customer.Id,
            Status = "Paid"
        };

        var invoice2Response = await _client.PostAsJsonAsync("/api/invoices", invoice2Dto);
        invoice2Response.StatusCode.Should().Be(HttpStatusCode.Created);
        var invoice2 = await invoice2Response.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice2.Should().NotBeNull();

        // Step 4: Verify Both Invoices Belong to Same Customer
        invoice1!.CustomerId.Should().Be(customer.Id);
        invoice2!.CustomerId.Should().Be(customer.Id);
        invoice1.Id.Should().NotBe(invoice2.Id);

        // Step 5: Get All Invoices and Verify Count
        var allInvoicesResponse = await _client.GetAsync("/api/invoices");
        allInvoicesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allInvoices = await allInvoicesResponse.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        allInvoices.Should().NotBeNull();
        allInvoices!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task UpdateAndDeleteProperty_ShouldWorkEndToEnd()
    {
        // Scenario: Create property, update it, then delete it (soft delete)

        // Step 1: Create Category
        var categoryDto = new CreateCategoryDto
        {
            Name = "Update Test Category",
            Description = "For update test"
        };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", categoryDto);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();

        // Step 2: Create Location
        var locationDto = new CreateLocationDto
        {
            CityName = "Update Test City",
            PlateCode = "88"
        };
        var locationResponse = await _client.PostAsJsonAsync("/api/locations", locationDto);
        var location = await locationResponse.Content.ReadFromJsonAsync<LocationDto>();

        // Step 3: Create Property
        var propertyDto = new CreatePropertyDto
        {
            Title = "Original Property",
            BlockNumber = "UPD-1",
            ParcelNumber = "UPD-P1",
            SquareMeters = 100.00m,
            Price = 300000.00m,
            CategoryId = category!.Id,
            LocationId = location!.Id,
            IsAvailable = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/properties", propertyDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProperty = await createResponse.Content.ReadFromJsonAsync<PropertyDto>();
        createdProperty.Should().NotBeNull();

        // Step 4: Update Property
        var updateDto = new UpdatePropertyDto
        {
            Title = "Updated Property",
            BlockNumber = "UPD-1-UPDATED",
            ParcelNumber = "UPD-P1-UPDATED",
            SquareMeters = 120.00m,
            Price = 350000.00m,
            CategoryId = category.Id,
            LocationId = location.Id,
            IsAvailable = false
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/properties/{createdProperty!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProperty = await updateResponse.Content.ReadFromJsonAsync<PropertyDto>();
        updatedProperty!.Title.Should().Be("Updated Property");
        updatedProperty.Price.Should().Be(350000.00m);
        updatedProperty.IsAvailable.Should().BeFalse();

        // Step 5: Delete Property (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/properties/{createdProperty.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 6: Verify Property is Deleted
        var getResponse = await _client.GetAsync($"/api/properties/{createdProperty.Id}");
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

