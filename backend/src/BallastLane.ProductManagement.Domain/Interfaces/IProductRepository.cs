using BallastLane.ProductManagement.Domain.Entities;

namespace BallastLane.ProductManagement.Domain.Interfaces
{
    /// <summary>
    /// IProductRepository Interface.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        public Task<IList<Product>> GetAllAsync();

        /// <summary>
        /// Get a paged list of products.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<(IList<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Product> GetByIdAsync(int id);

        /// <summary>
        /// Insert a new product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<Product> InsertAsync(Product product);

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<Product> UpdateAsync(Product product);

        /// <summary>
        /// Delete a product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(int id);
    }
}
