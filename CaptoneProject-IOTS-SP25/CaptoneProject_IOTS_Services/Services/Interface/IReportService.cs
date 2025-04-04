using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RatingDTO;
using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IReportService
    {
        public Task<ResponseDTO> GetReportPagination(int? filterStatus, PaginationRequest request);

        public Task<ResponseDTO> ApproveOrRejectReport(int reportId, bool isApprove);
    }
}
