
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class UserRepository : RepositoryBase<User>
    {
        private readonly string _loginRepository;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(
            string loginRepository
        )
        {
            _loginRepository = loginRepository;

        }

        public User GetLoginUser()
        {

            return new User
            {
                Id = 1
            };
        }

        public async Task<User> CheckLoginAsync(string email, string password)
        {
            var user = await _dbSet
                .Include(x => x.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<User> GetUserById(int id)
        {
            var user = await _dbSet
                .Include(x => x.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _dbSet
                .Include(x => x.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

    }
}
