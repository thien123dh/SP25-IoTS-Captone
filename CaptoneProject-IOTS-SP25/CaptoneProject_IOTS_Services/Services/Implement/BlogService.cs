using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.BlogCategoryDTO;
using CaptoneProject_IOTS_BOs.DTO.BlogDTO;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class BlogService : IBlogService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices _userServices;

        public BlogService(UnitOfWork unitOfWork,IUserServices userServices)
        {
            _unitOfWork ??= unitOfWork;
            this._userServices = userServices;
        }

        public Task<GenericResponseDTO<BlogResponeDetailDTO>> CreateOrUpdateBlog(int? id, CreateUpdateBlogDTO payload)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> CreateOrUpdateBlogCategory(int? id, CreateUpdateBlogCategoryDTO blogCategoryDTO)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> GetAllBlogCategory(string searchKeyword)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<BlogResponeDetailDTO>> GetBlogById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<BlogsCategory>> GetByBlogCategoryId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<PaginationResponseDTO<Blog>>> GetPagination(PaginationRequest payload)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<PaginationResponseDTO<BlogsCategory>>> GetPaginationBlogCategories(PaginationRequest paginate, int? statusFilter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> UpdateBlogCategoryStatus(int id, int IsActive)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<BlogResponeDetailDTO>> UpdateBlogStatus(int id, int status)
        {
            throw new NotImplementedException();
        }
    }
}
