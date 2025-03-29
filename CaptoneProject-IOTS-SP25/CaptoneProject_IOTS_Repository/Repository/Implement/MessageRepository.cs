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
    public class MessageRepository : RepositoryBase<Message>
    {
        public IQueryable<Message> GetAll()
        {
            return _dbSet.AsQueryable();
        }
    }
}
