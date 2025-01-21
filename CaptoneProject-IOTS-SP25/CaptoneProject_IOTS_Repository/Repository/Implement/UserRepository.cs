
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class UserRepository : RepositoryBase<User>
    {
        //private readonly string _loginRepository;
        public UserRepository(
            //string loginRepository
        )
        {
            //_loginRepository = loginRepository;

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
                .Include(x => x.Stores)
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
