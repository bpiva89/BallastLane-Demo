using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Interfaces;
using BallastLane.ProductManagement.Domain.Common;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;

namespace BallastLane.ProductManagement.Application.Services
{
    /// <summary>
    /// ProductService Class.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repository"></param>
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IList<ProductDto>>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            var dtos = products.Select(x => new ProductDto(x)).ToList();

            return Result.Success<IList<ProductDto>>(dtos);
        }

        /// <summary>
        /// Get a paged list of products.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<Result<PagedResult<ProductDto>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var (items, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);
            var dtos = items.Select(x => new ProductDto(x)).ToList();
            var pagedResult = new PagedResult<ProductDto>(dtos, totalCount, pageNumber, pageSize);

            return Result.Success(pagedResult);
        }

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<ProductDto>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return Result.Failure<ProductDto>("Invalid product identification.");

            var product = await _repository.GetByIdAsync(id);

            if (product is null)
                return Result.Failure<ProductDto>($"Product with id={id} was not found.");

            return Result.Success(new ProductDto(product));
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto)
        {
            if (dto is null)
                return Result.Failure<ProductDto>("Product data cannot be null.");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _repository.InsertAsync(product);

            return Result.Success(new ProductDto(result));
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result<ProductDto>> UpdateAsync(int id, UpdateProductDto dto)
        {
            if (id <= 0)
                return Result.Failure<ProductDto>("Invalid product identification.");

            if (dto is null)
                return Result.Failure<ProductDto>("Product data cannot be null.");

            var product = await _repository.GetByIdAsync(id);

            if (product is null)
                return Result.Failure<ProductDto>($"Product with id={id} was not found.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _repository.UpdateAsync(product);

            return Result.Success(new ProductDto(result));
        }

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAsync(int id)
        {
            if (id <= 0)
                return Result.Failure("Invalid product identification.");

            var product = await _repository.GetByIdAsync(id);

            if (product is null)
                return Result.Failure($"Product with id={id} was not found.");

            await _repository.DeleteAsync(id);

            return Result.Success();
        }
    }
}
