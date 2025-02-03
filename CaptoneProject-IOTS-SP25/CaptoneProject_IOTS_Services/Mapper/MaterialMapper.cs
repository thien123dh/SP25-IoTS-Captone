using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class MaterialMapper
    {
        private static IMapService<Material, MaterialDetailsResponseDTO> materialDetailsMapper = new MapService<Material, MaterialDetailsResponseDTO>();
        private static IMapService<CreateUpdateMaterialDTO, Material> materialMapper = new MapService<CreateUpdateMaterialDTO, Material>();
        private static IMapService<Material, MaterialItemDTO> materialItemMapper = new MapService<Material, MaterialItemDTO>();

        public static MaterialDetailsResponseDTO MapToMaterialDetailsResponseDTO(Material material, List<Attachment>? attachments)
        {
            MaterialDetailsResponseDTO res = materialDetailsMapper.MappingTo(material);

            res.ProductStatus = material.IsActive;

            res.MaterialAttachments = attachments?.Select(att => AttachmentMapper.MapToAttachmentDTO(att))?.ToList();

            return res;
        }

        public static Material MapToMaterial(CreateUpdateMaterialDTO source)
        {
            Material res = materialMapper.MappingTo(source);

            return res;
        }

        public static MaterialItemDTO MapToMaterialItemDTO(Material source)
        {
            MaterialItemDTO res = materialItemMapper.MappingTo(source);
            res.ProductStatus = source.IsActive;

            return res;
        }
    }
}
