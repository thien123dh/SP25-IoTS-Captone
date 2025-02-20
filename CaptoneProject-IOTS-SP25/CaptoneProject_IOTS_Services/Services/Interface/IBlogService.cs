using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.BlogDTO;
using CaptoneProject_IOTS_BOs.Models;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IBlogService
    {
        public Task<GenericResponseDTO<BlogResponeDetailDTO>> GetBlogById(int id);
        public Task<GenericResponseDTO<BlogResponeDetailDTO>> CreateOrUpdateBlog(int? id, CreateUpdateBlogDTO payload);
        public Task<GenericResponseDTO<PaginationResponseDTO<Blog>>> GetPagination(PaginationRequest payload);
        public Task<GenericResponseDTO<BlogResponeDetailDTO>> UpdateBlogStatus(int id, int status);
    }
}
