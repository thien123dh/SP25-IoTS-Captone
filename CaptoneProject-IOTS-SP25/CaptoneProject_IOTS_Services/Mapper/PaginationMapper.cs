using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class PaginationMapper<T, E>
    {
        public delegate E MappingItemFunction(T source);
        public static PaginationResponse<E> mappingTo(MappingItemFunction function, PaginationResponse<T> source)
        {
            return new PaginationResponse<E>
            {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                Data = source.Data?.Select(data => function(data))
            };
        }
    }
}
