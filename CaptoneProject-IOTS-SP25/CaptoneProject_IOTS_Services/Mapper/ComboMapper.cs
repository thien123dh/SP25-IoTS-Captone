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
        public static ComboDetailsResponseDTO MapToComboDetailsResponseDTO(Combo source)
        {
            return GenericMapper<Combo, ComboDetailsResponseDTO>.MapTo(source);
        }

        public static Combo MapToCombo(CreateUpdateComboDTO source)
        {
            return GenericMapper<CreateUpdateComboDTO, Combo>.MapTo(source);
        }
    }
}
