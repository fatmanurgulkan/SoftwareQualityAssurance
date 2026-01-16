using FluentAssertions;
using Moq;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;
using SoftwareQualityAssurance.Services;
using Xunit;

namespace SoftwareQualityAssurance.UnitTests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _service = new CustomerService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllCustomersAsync_ShouldReturnAllCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet@test.com" },
            new Customer { Id = 2, FirstName = "Ayşe", LastName = "Demir", Email = "ayse@test.com" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllCustomersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().FirstName.Should().Be("Ahmet");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var customer = new Customer { Id = 1, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        // Act
        var result = await _service.GetCustomerByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be("Ahmet");
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenCustomerNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.GetCustomerByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCustomerAsync_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Mehmet",
            LastName = "Kaya",
            Email = "mehmet@test.com",
            IdentityNumber = "12345678901",
            Balance = 50000,
            PhoneNumber = "05551234567"
        };

        var createdCustomer = new Customer
        {
            Id = 1,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            IdentityNumber = createDto.IdentityNumber,
            Balance = createDto.Balance,
            PhoneNumber = createDto.PhoneNumber,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>())).ReturnsAsync(createdCustomer);

        // Act
        var result = await _service.CreateCustomerAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Email.Should().Be(createDto.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            FirstName = "Mehmet",
            LastName = "Kaya",
            Email = "existing@test.com",
            IdentityNumber = "12345678901",
            Balance = 50000,
            PhoneNumber = "05551234567"
        };

        _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, null)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCustomerAsync(createDto));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerExists_ShouldUpdateCustomer()
    {
        // Arrange
        var existingCustomer = new Customer
        {
            Id = 1,
            FirstName = "Ahmet",
            LastName = "Yılmaz",
            Email = "ahmet@test.com",
            CreatedDate = DateTime.UtcNow.AddDays(-10)
        };

        var updateDto = new UpdateCustomerDto
        {
            FirstName = "Ahmet Updated",
            LastName = "Yılmaz Updated",
            Email = "ahmet.updated@test.com",
            IdentityNumber = "12345678901",
            Balance = 60000,
            PhoneNumber = "05551234567"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, 1)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>())).ReturnsAsync(existingCustomer);

        // Act
        var result = await _service.UpdateCustomerAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WhenCustomerNotExists_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto { FirstName = "Test", Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer?)null);

        // Act
        var result = await _service.UpdateCustomerAsync(999, updateDto);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var existingCustomer = new Customer { Id = 1, Email = "ahmet@test.com" };
        var updateDto = new UpdateCustomerDto { Email = "existing@test.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, 1)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateCustomerAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenCustomerExists_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteCustomerAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_WhenCustomerNotExists_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteCustomerAsync(999);

        // Assert
        result.Should().BeFalse();
    }
}

