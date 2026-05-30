using BallastLane.ProductManagement.Application.Dtos;

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
        public Task<IList<ProductDto>> GetAllAsync();

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ProductDto> GetByIdAsync(int id);

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ProductDto> CreateAsync(CreateProductDto dto);

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);

        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(int id);
    }
}
