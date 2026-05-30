using NHibernate;
using NHibernate.Linq;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;

namespace BallastLane.ProductManagement.Infrastructure.Repositories
{
    /// <summary>
    /// UserRepository Class.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ISessionFactory _factory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory"></param>
        public UserRepository(ISessionFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get user by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetByIdAsync(int id)
        {
            using (ISession session = _factory.OpenSession())
            {
                return await session.GetAsync<User>(id);
            }
        }

        /// <summary>
        /// Get user by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetByUsernameAsync(string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);

            using (ISession session = _factory.OpenSession())
            {
                return await session.Query<User>()
                    .Where(x => x.Username == username)
                    .FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// Insert a new user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> InsertAsync(User user)
        {
            using (var session = _factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                await session.SaveAsync(user);
                await tx.CommitAsync();

                return user;
            }
        }
    }
}
