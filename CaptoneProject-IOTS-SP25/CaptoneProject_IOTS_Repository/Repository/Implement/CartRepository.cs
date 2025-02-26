using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Repository.Repository.Implement
{
    public class CartRepository : RepositoryBase<CartItem>
    {
        public CartItem? GetById(int id) => _dbSet
            .Include(item => item.IosDeviceNavigation)
            .ThenInclude(iot => iot.StoreNavigation)
            .Include(item => item.LabNavigation)
            .ThenInclude(lab => lab.CreatedByNavigation)
            .Include(item => item.ComboNavigation)
            .ThenInclude(combo => combo.StoreNavigation)
            .SingleOrDefault(item => item.Id == id);
        public CartItem? GetCartItemByProductId(int userId, int productId, int productType)
        {
            return _dbSet
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .Include(item => item.LabNavigation)
                .SingleOrDefault(
                item => (item.CreatedBy == userId) &&
                (
                    (productType == (int)ProductTypeEnum.IOT_DEVICE && item.IosDeviceId == productId)
                    || (productType == (int)ProductTypeEnum.COMBO && item.ComboId == productId)
                    || (productType == (int)ProductTypeEnum.LAB && item.LabId == productId)
                )
            );
        }

        public List<CartItem>? GetAllCartItemsByUserId(int userId, int productType)
        {
            return _dbSet
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .Include(item => item.LabNavigation)
                .Where(item => item.CreatedBy == userId && item.ProductType == productType)?.ToList();
        }

        public List<CartItem>? GetCartItemsListByParentId(int parentId)
        {
            return _dbSet
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .Include(item => item.LabNavigation)
                .Where(item => item.ParentCartItemId == parentId)?.ToList();
        }

        public List<CartItem>? GetSubItemsListByUserId(int userId)
        {
            return _dbSet
                .Include(item => item.IosDeviceNavigation)
                .Include(item => item.ComboNavigation)
                .Include(item => item.LabNavigation)
                .Where(item => item.CreatedBy == userId && item.ParentCartItemId != null)?.ToList();

        }
    }
}
