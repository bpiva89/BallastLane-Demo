using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Services;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BallastLane.ProductManagement.Tests.Unit
{
    /// <summary>
    /// Unit tests for ProductService.
    /// Repository is mocked so tests focus on business logic only.
    /// </summary>
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _service = new ProductService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Widget A", Description = "Desc", Price = 9.99m, Stock = 10, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = "Widget B", Description = "Desc", Price = 19.99m, Stock = 5, CreatedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value[0].Name.Should().Be("Widget A");
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Widget A", Description = "Desc", Price = 9.99m, Stock = 10, CreatedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetPagedAsync(1, 10)).ReturnsAsync((products, 1));

            // Act
            var result = await _service.GetPagedAsync(1, 10);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.TotalCount.Should().Be(1);
            result.Value.PageNumber.Should().Be(1);
            result.Value.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnDto()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Widget A", Description = "Desc", Price = 9.99m, Stock = 10, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Widget A");
            result.Value.Price.Should().Be(9.99m);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ShouldReturnFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product)null!);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("was not found");
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdIsZeroOrNegative_ShouldReturnFailure()
        {
            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("Invalid product identification");
        }

        [Fact]
        public async Task CreateAsync_ShouldBuildEntityAndCallRepository()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "New Widget", Description = "Desc", Price = 15m, Stock = 20 };
            _repositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => { p.Id = 42; return p; });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(42);
            result.Value.Name.Should().Be("New Widget");
            _repositoryMock.Verify(r => r.InsertAsync(It.Is<Product>(p =>
                p.Name == "New Widget" && p.Stock == 20
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ShouldReturnFailure()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product)null!);
            var dto = new UpdateProductDto { Name = "Updated", Description = "Desc", Price = 5m, Stock = 1 };

            // Act
            var result = await _service.UpdateAsync(999, dto);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("was not found");
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_ShouldUpdateFields()
        {
            // Arrange
            var existing = new Product { Id = 1, Name = "Old", Description = "Old Desc", Price = 5m, Stock = 10, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(existing);

            var dto = new UpdateProductDto { Name = "New Name", Description = "New Desc", Price = 25m, Stock = 30 };

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
                p.Name == "New Name" && p.Price == 25m && p.Stock == 30
            )), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenIdIsZeroOrNegative_ShouldReturnFailure()
        {
            // Act
            var result = await _service.DeleteAsync(0);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("Invalid product identification");
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldCallRepository()
        {
            // Arrange
            var existing = new Product { Id = 1, Name = "Widget A", Description = "Desc", Price = 9.99m, Stock = 10, CreatedAt = DateTime.UtcNow };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}
