using BallastLane.ProductManagement.Domain.Entities;

namespace BallastLane.ProductManagement.Domain.Interfaces
{
    /// <summary>
    /// IUserRepository Interface.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<User> GetByIdAsync(int id);

        /// <summary>
        /// Get user by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<User> GetByUsernameAsync(string username);

        /// <summary>
        /// Insert a new user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User> InsertAsync(User user);
    }
}
