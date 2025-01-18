using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IActivityLogService
    {
        public ResponseDTO GetAllActivityLogTypes();
        public Task<ResponseDTO> GetPaginationActivityLog(PaginationRequest payload, int? entityId, int? entityType, int? userId);
        public Task<ResponseDTO> CreateActivityLog(CreateActivityLogDTO source);
        public Task<ResponseDTO> CreateUserHistoryTrackingActivityLog(string action, string target, string? metaData);

    }
}
