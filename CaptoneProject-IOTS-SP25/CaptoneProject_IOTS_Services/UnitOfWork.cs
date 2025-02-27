using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Base;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service
{
    public class UnitOfWork
    {
        public virtual AccountMembershipPackageRepository AccountMembershipPackageRepository => new AccountMembershipPackageRepository();
        public virtual UserRepository UserRepository => new UserRepository();
        public virtual MaterialCategoryRepository MaterialCategoryRepository => new MaterialCategoryRepository();
        public virtual MembershipPackageRepository MembershipPackageRepository => new MembershipPackageRepository();
        public virtual IotsDeviceRepository IotsDeviceRepository => new IotsDeviceRepository();
        public virtual UserRoleRepository UserRoleRepository => new UserRoleRepository();
        public virtual WalletRepository WalletRepository => new WalletRepository();
        public virtual BlogCategoryRepository BlogCategoryRepository => new BlogCategoryRepository();
        public virtual BlogRepository BlogRepository => new BlogRepository();
        public virtual StoreRepository StoreRepository => new StoreRepository();
        public virtual CartRepository CartRepository => new CartRepository();
        public virtual LabRepository LabRepository => new LabRepository();
        public virtual ComboRepository ComboRepository => new ComboRepository();
        public virtual DeviceComboRepository DeviceComboRepository => new DeviceComboRepository();
        public virtual OrderRepository OrderRepository => new OrderRepository();
        public virtual OrderDetailRepository OrderDetailRepository => new OrderDetailRepository();
    }
}
