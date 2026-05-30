using BallastLane.ProductManagement.API.Controllers;
using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Interfaces;
using BallastLane.ProductManagement.Domain.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BallastLane.ProductManagement.Tests.Unit
{
    public class ProductsControllerTests
    {
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly Mock<IProductService> _serviceMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _serviceMock = new Mock<IProductService>();
            _controller = new ProductsController(_loggerMock.Object, _serviceMock.Object);
        }

        [Fact]
        public async Task GetPaged_ShouldReturn200WithPagedProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 },
                new ProductDto { Id = 2, Name = "Widget B", Description = "An advanced widget", Price = 19.99m, Stock = 50 }
            };
            var paged = new PagedResult<ProductDto>(products, 2, 1, 10);
            _serviceMock.Setup(s => s.GetPagedAsync(1, 10)).ReturnsAsync(Result.Success(paged));

            // Act
            var result = await _controller.GetPaged(1, 10);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            var response = ok.Value as PagedResult<ProductDto>;
            response.Should().NotBeNull();
            response!.Items.Should().HaveCount(2);
            response.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task GetAll_ShouldReturn200WithProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 },
                new ProductDto { Id = 2, Name = "Widget B", Description = "An advanced widget", Price = 19.99m, Stock = 50 }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(Result.Success<IList<ProductDto>>(products));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            var response = ok.Value as IList<ProductDto>;
            response.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_WhenExists_ShouldReturn200()
        {
            // Arrange
            var product = new ProductDto { Id = 1, Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(Result.Success(product));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_WhenNotFound_ShouldReturn404()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync(Result.Failure<ProductDto>("Product not found."));

            // Act
            var result = await _controller.GetById(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturn201WithCreatedProduct()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 };
            var created = new ProductDto { Id = 10, Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 };
            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(Result.Success(created));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAt = result.Result as CreatedAtActionResult;
            createdAt.Should().NotBeNull();
            createdAt!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Update_WhenNotFound_ShouldReturn404()
        {
            // Arrange
            var dto = new UpdateProductDto { Name = "Widget A", Description = "A basic widget", Price = 9.99m, Stock = 100 };
            _serviceMock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(Result.Failure<ProductDto>("Product was not found."));

            // Act
            var result = await _controller.Update(999, dto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Update_WhenExists_ShouldReturn200()
        {
            // Arrange
            var dto = new UpdateProductDto { Name = "Widget A Updated", Description = "Updated widget", Price = 12.99m, Stock = 80 };
            var updated = new ProductDto { Id = 1, Name = "Widget A Updated", Description = "Updated widget", Price = 12.99m, Stock = 80 };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(Result.Success(updated));

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_WhenNotFound_ShouldReturn404()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(Result.Failure("Product not found."));

            // Act
            var result = await _controller.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_WhenExists_ShouldReturn204()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
