using AutoMapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MapService<TSource, EDestination>: IMapService<TSource, EDestination>
    {
        private readonly IMapper _mapper;

        public MapService()
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<TSource, EDestination>(); });
            _mapper = config.CreateMapper();
        }

        public EDestination MappingTo(TSource source)
        {
            return _mapper.Map<EDestination>(source);
        }
    }
}
