using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftwareQualityAssurance.Data;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;
using SoftwareQualityAssurance.Services;
using Xunit;

namespace SoftwareQualityAssurance.UnitTests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IRepository<Invoice>> _mockInvoiceRepository;
    private readonly Mock<IRepository<Customer>> _mockCustomerRepository;
    private readonly ApplicationDbContext _context;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        _mockInvoiceRepository = new Mock<IRepository<Invoice>>();
        _mockCustomerRepository = new Mock<IRepository<Customer>>();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        
        _service = new InvoiceService(_mockInvoiceRepository.Object, _mockCustomerRepository.Object, _context);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithValidData_ShouldCreateInvoice()
    {
        // Arrange
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = 5000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = 1,
            Status = "Pending"
        };

        // Add customer to context for Include to work
        var customer = new Customer
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            IdentityNumber = "12345678901",
            Balance = 10000,
            PhoneNumber = "5551234567",
            CreatedDate = DateTime.UtcNow
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var createdInvoice = new Invoice
        {
            Id = 1,
            SerialNumber = createDto.SerialNumber,
            TotalAmount = createDto.TotalAmount,
            InvoiceDate = createDto.InvoiceDate,
            CustomerId = createDto.CustomerId,
            Status = createDto.Status,
            CreatedDate = DateTime.UtcNow
        };

        // Add invoice to context for Include to work
        _context.Invoices.Add(createdInvoice);
        await _context.SaveChangesAsync();

        _mockCustomerRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockInvoiceRepository.Setup(r => r.AddAsync(It.IsAny<Invoice>())).ReturnsAsync(createdInvoice);

        // Act
        var result = await _service.CreateInvoiceAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(5000.00m);
        _mockInvoiceRepository.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = 0,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = 1,
            Status = "Pending"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateInvoiceAsync(createDto));
        _mockInvoiceRepository.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = -100,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = 1,
            Status = "Pending"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateInvoiceAsync(createDto));
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithNonExistentCustomer_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateInvoiceDto
        {
            SerialNumber = "INV-001",
            TotalAmount = 5000.00m,
            InvoiceDate = DateTime.UtcNow,
            CustomerId = 999,
            Status = "Pending"
        };

        _mockCustomerRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateInvoiceAsync(createDto));
    }

    [Fact]
    public async Task UpdateInvoiceAsync_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var existingInvoice = new Invoice { Id = 1, TotalAmount = 5000 };
        var updateDto = new UpdateInvoiceDto { TotalAmount = 0 };

        _mockInvoiceRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingInvoice);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateInvoiceAsync(1, updateDto));
    }
}

