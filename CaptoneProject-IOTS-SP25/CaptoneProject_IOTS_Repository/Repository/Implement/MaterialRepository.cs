using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
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
    public class MaterialRepository : RepositoryBase<Material>
    {
        public Material GetById(int id)
        {
            return _dbSet
                .Include(item => item.Category)
                .Include(item => item.StoreNavigation)
                .SingleOrDefault(item => item.Id == id);
        }
        public async Task<Material> GetCategoryMaterialById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<Material>> GetAllMaterialCaterial()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
