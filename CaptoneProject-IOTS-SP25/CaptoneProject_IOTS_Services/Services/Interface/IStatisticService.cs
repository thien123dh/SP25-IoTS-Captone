using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IStatisticService
    {
        public Task<ResponseDTO> GetAdminStatistic(StatisticRequest request);

        public Task<ResponseDTO> GetTrainerStatistic(StatisticRequest request);

        public Task<ResponseDTO> GetStoreStatistic(StatisticRequest request);
    }
}
