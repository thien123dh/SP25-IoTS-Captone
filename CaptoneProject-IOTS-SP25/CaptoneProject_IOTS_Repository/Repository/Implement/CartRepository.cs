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
        public CartItem? GetCartItemByProductId(int userId, int productId, int productType)
        {
            return _dbSet.SingleOrDefault(
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
            return _dbSet.Where(item => item.CreatedBy == userId && item.ProductType == productType)?.ToList();
        }

        public List<CartItem>? GetCartItemsListByParentId(int parentId)
        {
            return _dbSet
                .Include(item => item.LabNavigation)
                .Where(item => item.ParentCartItemId == parentId)?.ToList();
        }
    }
}
