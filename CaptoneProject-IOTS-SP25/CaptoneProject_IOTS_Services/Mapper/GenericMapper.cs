using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class GenericMapper<S, D>
    {
        private static readonly IMapService<S, D> mapper = new MapService<S, D>();

        public static D MapTo(S source)
        {
            return mapper.MappingTo(source);
        }
    }
}
