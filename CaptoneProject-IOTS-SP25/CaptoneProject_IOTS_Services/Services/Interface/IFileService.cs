using CaptoneProject_IOTS_BOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.FileDTO.FileDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IFileService
    {
        Task<GenericResponseDTO<FileResponseDTO>> UploadFile(IFormFile files);
    }
}
