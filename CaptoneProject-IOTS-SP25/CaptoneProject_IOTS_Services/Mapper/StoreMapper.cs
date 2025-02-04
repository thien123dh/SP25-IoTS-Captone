using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class StoreMapper
    {
        private readonly static IMapService<Store, StoreResponseDTO> storeResponseMapper = new MapService<Store, StoreResponseDTO>();
        public static StoreDetailsResponseDTO MapToStoreDetailsResponseDTO(Store store)
        {
            return new StoreDetailsResponseDTO
            {
                Id = store.Id,
                Name = store.Name,
                Description = store.Description,
                OwnerId = store.OwnerId,
                ImageUrl = store.ImageUrl,
                CreatedBy = store.CreatedBy,
                CreatedDate = store.CreatedDate,
                Summary = store.Summary,
                ContactNumber = store.ContactNumber,
                Address = store.Address,
                StoreAttachments = store.StoreAttachmentsNavigation?.Select(sa => new StoreAttachmentResponseDTO
                {
                    Id = sa.Id,
                    ImageUrl = sa.ImageUrl,
                    CreatedDate = sa.CreatedDate,
                    CreatedBy = sa.createdBy,
                    StoreId = sa.StoreId
                }).ToList()
            };
        }

        public static StoreResponseDTO MapToStoreResponse(Store source)
        {
            var des = storeResponseMapper.MappingTo(source);
            
            des.OwnerName = source.Owner.Fullname;

            return des;
        }

    }
}
