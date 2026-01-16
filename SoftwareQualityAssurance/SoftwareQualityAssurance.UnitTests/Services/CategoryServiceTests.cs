using FluentAssertions;
using Moq;
using SoftwareQualityAssurance.DTOs;
using SoftwareQualityAssurance.Models;
using SoftwareQualityAssurance.Repositories;
using SoftwareQualityAssurance.Services;
using Xunit;

namespace SoftwareQualityAssurance.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Category>> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<IRepository<Category>>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Apartment", Description = "Residential" },
            new Category { Id = 2, Name = "Villa", Description = "Luxury" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Office", Description = "Commercial" };
        var createdCategory = new Category { Id = 1, Name = createDto.Name, Description = createDto.Description };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Category>())).ReturnsAsync(createdCategory);

        // Act
        var result = await _service.CreateCategoryAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Office");
    }
}

