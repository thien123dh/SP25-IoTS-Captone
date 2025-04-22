using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_Repository.Base;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class MaterialCategoryRepository : RepositoryBase<MaterialCategory>
    {
        public async Task<MaterialCategory> GetCategoryMaterialById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<MaterialCategory>> GetAllMaterialCaterial()
        {
            return await _dbSet.ToListAsync();
        }

        public bool Any(Expression<Func<MaterialCategory, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }
    }
}
