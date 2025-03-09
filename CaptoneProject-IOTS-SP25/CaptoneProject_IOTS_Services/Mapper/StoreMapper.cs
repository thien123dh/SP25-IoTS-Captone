using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
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
            var res = GenericMapper<Store, StoreDetailsResponseDTO>.MapTo(store);

            res.StoreAttachments = store.StoreAttachmentsNavigation?.Select(sa => new StoreAttachmentResponseDTO
            {
                Id = sa.Id,
                ImageUrl = sa?.ImageUrl,
                CreatedDate = sa?.CreatedDate,
                CreatedBy = sa?.createdBy,
                StoreId = sa.StoreId
            }).ToList();

            return res;
        }

        public static StoreResponseDTO MapToStoreResponse(Store source)
        {
            var des = storeResponseMapper.MappingTo(source);
            
            des.OwnerName = source.Owner.Fullname;

            return des;
        }

    }
}
