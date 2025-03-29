using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.CommonDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class CommonService : ICommonService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly int DEFAULT_SELECT_TOP = 20;
        public CommonService(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<GenericResponseDTO<CommonSearchResponseDTO>> RelativeSearchApplication(CommonSearchRequestDTO request)
        {
            var devices = unitOfWork.IotsDeviceRepository.Search(
                item => request.Keyword.ToLower().Contains(item.Name) && item.IsActive > 0
            ).OrderByDescending(item => item.CreatedDate).Take(DEFAULT_SELECT_TOP).ToList();

            var combos = unitOfWork.ComboRepository.Search(
                item => request.Keyword.ToLower().Contains(item.Name) && item.IsActive > 0
            ).OrderByDescending(item => item.CreatedDate).Take(DEFAULT_SELECT_TOP).ToList();

            var res = new CommonSearchResponseDTO
            {
                IotDevices = devices,
                Combos = combos
            };

            return ResponseService<CommonSearchResponseDTO>.OK(res);
        }
    }
}
