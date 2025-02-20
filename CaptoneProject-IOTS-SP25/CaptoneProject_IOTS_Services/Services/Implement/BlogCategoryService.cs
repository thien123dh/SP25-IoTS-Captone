using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.BlogCategoryDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using CaptoneProject_IOTS_Service.ResponseService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_Service.Business;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class BlogCategoryService : IBlogService
    {
        private readonly UnitOfWork _unitOfWork;

        public BlogCategoryService(UnitOfWork unitOfWork)
        {
            _unitOfWork ??= unitOfWork;
        }

        public async Task<ResponseDTO> CreateOrUpdateBlogCategory(int? id, CreateUpdateBlogCategoryDTO blogCategoryDTO)
        {
           BlogsCategory blogCategory = (id == null) ? new BlogsCategory() : await _unitOfWork.BlogCategoryRepository.GetByIdAsync((int)id);

            if (blogCategory == null)
                return ResponseService<Object>.NotFound(ExceptionMessage.BLOG_CATEGORY_NOTFOUND);

            try
            {
                blogCategory.Label = blogCategoryDTO.Label;
                blogCategory.IsActive = 1;

                BlogsCategory? response;

                if (id > 0) //Update
                    response = _unitOfWork.BlogCategoryRepository.Update(blogCategory);
                else //Create
                    response = _unitOfWork.BlogCategoryRepository.Create(blogCategory);

                return await GetByBlogCategoryId(response.Id);
            }
            catch (Exception ex)
            {
                return ResponseService<Object>.BadRequest(ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllBlogCategory(string searchKeyword)
        {
            PaginationResponseDTO<BlogsCategory> res = _unitOfWork.BlogCategoryRepository.GetPaginate(
                filter: m => m.Label.Contains(searchKeyword) && m.IsActive > 0,
                orderBy: null,
                includeProperties: "",
                pageIndex: 0,
                pageSize: 500000
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = res
            };
        }       

        public async Task<GenericResponseDTO<BlogsCategory>> GetByBlogCategoryId(int id)
        {
            var res = _unitOfWork.BlogCategoryRepository.GetById(id);

            if (res == null)
                return ResponseService<BlogsCategory>.NotFound("Not Found");

            return ResponseService<BlogsCategory>.OK(res);
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<BlogsCategory>>> GetPaginationBlogCategories(PaginationRequest paginate, int? statusFilter)
        {
            PaginationResponseDTO<BlogsCategory> res = _unitOfWork.BlogCategoryRepository.GetPaginate(
                filter: m => m.Label.Contains(paginate.SearchKeyword)
                    && (statusFilter == null || m.IsActive == statusFilter),
                orderBy: orderBy => orderBy.OrderByDescending(item => item.Id),
                includeProperties: "",
                pageIndex: paginate.PageIndex,
                pageSize: paginate.PageSize
            );

            return ResponseService<PaginationResponseDTO<BlogsCategory>>.OK(res);
        }

        public async Task<ResponseDTO> UpdateBlogCategoryStatus(int id, int IsActive)
        {
            BlogsCategory res = _unitOfWork.BlogCategoryRepository.GetById(id);

            if (res == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_CATEGORY_NOTFOUND
                };

            res.IsActive = IsActive;

            var response = _unitOfWork.BlogCategoryRepository.Update(res);

            return await GetByBlogCategoryId(response.Id);
        }
    }
}
