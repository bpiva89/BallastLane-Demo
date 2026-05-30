using NHibernate;
using NHibernate.Linq;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;

namespace BallastLane.ProductManagement.Infrastructure.Repositories
{
    /// <summary>
    /// ProductRepository Class.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ISessionFactory _factory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory"></param>
        public ProductRepository(ISessionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Product>> GetAllAsync()
        {
            using (ISession session = _factory.OpenSession())
            {
                return await session.Query<Product>()
                    .OrderBy(x => x.Id)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Get a paged list of products.
        /// </summary>
        public async Task<(IList<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            using (ISession session = _factory.OpenSession())
            {
                var query = session.Query<Product>();
                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderBy(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
        }

        /// <summary>
        /// Get product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product> GetByIdAsync(int id)
        {
            using (ISession session = _factory.OpenSession())
            {
                return await session.GetAsync<Product>(id);
            }
        }

        /// <summary>
        /// Insert a new product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product> InsertAsync(Product product)
        {
            using (var session = _factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                await session.SaveAsync(product);
                await tx.CommitAsync();

                return product;
            }
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product> UpdateAsync(Product product)
        {
            using (var session = _factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                await session.UpdateAsync(product);
                await tx.CommitAsync();

                return product;
            }
        }

        /// <summary>
        /// Delete a product by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            using (var session = _factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var product = await session.GetAsync<Product>(id);

                if (product is not null)
                    await session.DeleteAsync(product);

                await tx.CommitAsync();
            }
        }
    }
}
