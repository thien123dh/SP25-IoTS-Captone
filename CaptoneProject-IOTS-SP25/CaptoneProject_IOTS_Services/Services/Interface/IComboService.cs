using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IComboService
    {
        public Task<PaginationResponseDTO<ComboItemDTO>> GetPaginationCombos(PaginationRequest request);

        public Task<ComboDetailsResponseDTO> GetComboDetailsById(int comboId);

        public Task<GenericResponseDTO<ComboDetailsResponseDTO>> CreateOrUpdateCombo(int? id, CreateUpdateComboDTO payload);

        public Task<ResponseDTO> ActivateOrDeactiveCombo(int comboId, bool isActivate);
    }
}
