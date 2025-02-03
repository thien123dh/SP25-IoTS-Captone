using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IAttachmentsService
    {
        public Task<ResponseDTO> CreateOrUpdateAttachments(int entityId, int entityType, List<AttachmentsDTO>? payload);
    }
}
