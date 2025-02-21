using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.BlogCategoryDTO;
using CaptoneProject_IOTS_BOs.DTO.BlogDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IBlogCategoryService
    {
        Task<ResponseDTO> CreateOrUpdateBlogCategory(int? id, CreateUpdateBlogCategoryDTO blogCategoryDTO);
        Task<ResponseDTO> UpdateBlogCategoryStatus(int id, int IsActive);
        Task<ResponseDTO> GetAllBlogCategory(string searchKeyword);
        Task<GenericResponseDTO<PaginationResponseDTO<BlogsCategory>>> GetPaginationBlogCategories(PaginationRequest paginate, int? statusFilter);
        Task<GenericResponseDTO<BlogsCategory>> GetByBlogCategoryId(int id);
    }
}
