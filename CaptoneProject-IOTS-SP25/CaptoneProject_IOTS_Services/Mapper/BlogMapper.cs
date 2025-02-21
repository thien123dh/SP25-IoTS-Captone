using CaptoneProject_IOTS_BOs.DTO.BlogDTO;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public class BlogMapper
    {
        private static readonly IMapService<Blog, BlogResponeDetailDTO> blogMapper = new MapService<Blog, BlogResponeDetailDTO>();
        private static readonly IMapService<CreateUpdateBlogDTO, Blog> blogSaveMapper = new MapService<CreateUpdateBlogDTO, Blog>();

        public static BlogResponeDetailDTO MapToBlogDetailsDTO(Blog source)
        {
            return blogMapper.MappingTo(source);
        }

        public static Blog MapToBlog(CreateUpdateBlogDTO source)
        {
            return blogSaveMapper.MappingTo(source);
        }
    }
}
