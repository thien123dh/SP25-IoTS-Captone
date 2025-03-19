using Azure.Storage.Blobs;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.FileDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.FileDTO.FileDTO;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class FileService : IFileService
    {
        private readonly string bucket;
        public FileService(string bucket)
        {
            this.bucket = bucket;
        }
        public async Task<GenericResponseDTO<FileResponseDTO>> UploadFile(IFormFile file)
        {
            var cancellation = new CancellationTokenSource();
            
            string guid = $"{Guid.NewGuid().ToString()}.png";
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var task = new FirebaseStorage(bucket)
                    .Child("image")
                    .Child(guid + ".png")
                    .PutAsync(memoryStream, cancellation.Token);

                try
                {
                    string downloadUrl = await task;

                    return ResponseService<FileResponseDTO>.OK(new FileResponseDTO
                    {
                        FileName = file.FileName,
                        Id = downloadUrl
                    });
                }
                catch (Exception ex)
                {
                    return ResponseService<FileResponseDTO>.BadRequest(ex.Message);
                }
            }

        }
    }
}
