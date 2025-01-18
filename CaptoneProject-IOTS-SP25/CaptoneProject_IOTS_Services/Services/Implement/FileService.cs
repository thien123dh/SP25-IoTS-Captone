using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.FileDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.FileDTO.FileDTO;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class FileService : IFileService
    {
        BlobServiceClient blobService;
        BlobContainerClient blobContainerClient;
        private readonly string loginUser = "matt";
        public FileService(
            BlobServiceClient blobService
        )
        {
            this.blobService = blobService;
            this.blobContainerClient = this.blobService.GetBlobContainerClient("iot-trading-system-storage-container");
        }
        public async Task<GenericResponseDTO<FileResponseDTO>> UploadFile(IFormFile file)
        {
            //var azureResponse = new List<BlobContentInfo>();

            string guid = $"{Guid.NewGuid().ToString()}.png";
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var _client = await blobContainerClient.UploadBlobAsync(guid, memoryStream, default);
            }

            return new GenericResponseDTO<FileResponseDTO>
            {
                IsSuccess = true,
                Message = "File upload successfully",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = new FileResponseDTO
                {
                    Id = guid,
                    FileName = file.FileName,
                }
            };
        }
    }
}
