using CaptoneProject_IOTS_Repository.Repository.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service
{
    public class UnitOfWork
    {
        private MaterialCategoryRepository _materialCategoryRepository;
        private MaterialGroupCategoryRepository _materialGroupCategoryRepository;
        private MaterialRepository _materialRepository;
        public UnitOfWork()
        {
        }

        public MaterialCategoryRepository MaterialCategoryRepository
        {
            get
            {
                return _materialCategoryRepository ??= new MaterialCategoryRepository();
            }
        }

        public MaterialRepository MaterialRepository
        {
            get
            {
                return _materialRepository ??= new MaterialRepository();
            }
        }

        public MaterialGroupCategoryRepository MaterialGroupCategoryRepository
        {
            get
            {
                return _materialGroupCategoryRepository ??= new MaterialGroupCategoryRepository();
            }
        }
    }
}
