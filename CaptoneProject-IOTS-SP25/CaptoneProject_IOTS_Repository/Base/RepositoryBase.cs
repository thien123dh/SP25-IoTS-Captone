using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Repository.Base
{
    public class RepositoryBase<T> where T : class
    {
        protected readonly IoTTraddingSystemContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase()
        {
            _context ??= new IoTTraddingSystemContext();
            _dbSet = _context.Set<T>();
        }

        #region Separating asign entity and save operators

        public RepositoryBase(IoTTraddingSystemContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void PrepareCreate(T entity)
        {
            _dbSet.Add(entity);
        }

        public void PrepareUpdate(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }

        public void PrepareRemove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion Separating asign entity and save operators

        public virtual PaginationResponse<T> GetPaginate(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string? includeProperties = "",
            int pageIndex = 0,
            int pageSize = 0)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            //Get total count before pagination
            int totalCount = query.Count();

            //if (pageIndex.HasValue && pageSize.HasValue)
            //{
            int validPageIndex = pageIndex > 0 ? pageIndex - 1 : 0;
            int validPageSize = pageSize > 0 ? pageSize : 10;

            query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            //}

            return
                new PaginationResponse<T>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Data = query.AsEnumerable()
                };
        }

        public List<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public T Create(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public async Task<int> CreateAsync(IEnumerable<T> entity)
        {
            await _dbSet.AddRangeAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public T Update(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            _context.SaveChanges();

            return entity;
        }

        public async Task<int> UpdateAsync(IEnumerable<T> entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public bool Remove(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public T GetById(string code)
        {
            return _dbSet.Find(code);
        }

        public async Task<T> GetByIdAsync(string code)
        {
            return await _dbSet.FindAsync(code);
        }

        public T GetById(Guid code)
        {
            return _dbSet.Find(code);
        }

        public async Task<T> GetByIdAsync(Guid code)
        {
            return await _dbSet.FindAsync(code);
        }

        public async Task SaveChange()
        {
            await _context.SaveChangesAsync();
        }
    }
}
