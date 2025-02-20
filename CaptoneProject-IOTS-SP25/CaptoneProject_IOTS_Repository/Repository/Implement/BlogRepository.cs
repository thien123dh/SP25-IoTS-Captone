using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class BlogRepository : RepositoryBase<Blog>
    {
        public async Task<Blog> GetBlogById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<Blog>> GetAllBlog()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
