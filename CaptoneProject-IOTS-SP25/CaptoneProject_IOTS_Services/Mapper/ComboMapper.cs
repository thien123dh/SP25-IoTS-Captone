using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class ComboMapper
    {
        private readonly static IMapService<Combo, ComboDetailsResponseDTO> comboDetailsMapper = new MapService<Combo, ComboDetailsResponseDTO>();
        private readonly static IMapService<CreateUpdateComboDTO, Combo> comboMapper = new MapService<CreateUpdateComboDTO, Combo>();

        public static ComboDetailsResponseDTO MapToComboDetailsResponseDTO(Combo source)
        {
            return comboDetailsMapper.MappingTo(source);
        }

        public static Combo MapToCombo(CreateUpdateComboDTO source)
        {
            return comboMapper.MappingTo(source);
        }
    }
}
