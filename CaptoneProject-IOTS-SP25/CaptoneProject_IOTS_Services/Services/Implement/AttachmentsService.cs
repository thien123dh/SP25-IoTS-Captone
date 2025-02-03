using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class AttachmentsService : IAttachmentsService
    {
        private readonly AttachmentRepository attachmentRepository;
        private readonly IUserServices userServices;

        public AttachmentsService(AttachmentRepository attachmentRepository,
            IUserServices userServices)
        {
            this.attachmentRepository = attachmentRepository;
            this.userServices = userServices;
        }

        public async Task<ResponseDTO> CreateOrUpdateAttachments(int entityId, int entityType, List<AttachmentsDTO>? payload)
        {
            List<Attachment>? dbAttachments = attachmentRepository.GetAttachmentsByEntityId(entityId, entityType);

            //Not exist in payload
            var removedItems = dbAttachments?.Where(dbAtt => payload?.FirstOrDefault(item => item.Id == dbAtt.Id) == null).ToList();

            try
            {
                if (removedItems != null)
                    await attachmentRepository.RemoveAsync(removedItems);

                int? loginUserId = userServices.GetLoginUserId();

                var insertItems = payload?.Where(item => !(item.Id > 0)).Select(item => new Attachment
                {
                    ImageUrl = item.ImageUrl,
                    EntityId = entityId,
                    EntityType = entityType,
                    CreatedBy = loginUserId,
                    MetaData = item.MetaData
                }).ToList();

                if (insertItems != null)
                    await attachmentRepository.CreateAsync(insertItems);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
            }
        }
    }
}
