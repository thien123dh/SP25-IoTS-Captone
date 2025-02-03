using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class AttachmentMapper
    {
        public static AttachmentsDTO MapToAttachmentDTO(Attachment source)
        {
            return new AttachmentsDTO
            {
                Id = source.Id,
                ImageUrl = source.ImageUrl,
                CreatedDate = source.CreatedDate,
                MetaData = source.MetaData
            };
        }
    }
}
