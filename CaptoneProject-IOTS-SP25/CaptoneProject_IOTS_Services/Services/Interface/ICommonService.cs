using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.CommonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ICommonService
    {
        public Task<GenericResponseDTO<CommonSearchResponseDTO>> RelativeSearchApplication(CommonSearchRequestDTO request);
    }
}
