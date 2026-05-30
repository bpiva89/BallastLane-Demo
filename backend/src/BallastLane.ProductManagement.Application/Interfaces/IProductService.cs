using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Domain.Common;

namespace BallastLane.ProductManagement.Application.Interfaces
{
    /// <summary>
    /// IProductService Interface.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        public Task<Result<IList<ProductDto>>> GetAllAsync();

        /// <summary>
        /// Get a paged list of products.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<Result<PagedResult<ProductDto>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Result<ProductDto>> GetByIdAsync(int id);

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<Result<ProductDto>> CreateAsync(CreateProductDto dto);

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<Result<ProductDto>> UpdateAsync(int id, UpdateProductDto dto);

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Result> DeleteAsync(int id);
    }
}
