using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMapService<TSource, EDestination>
    {
        public EDestination MappingTo(TSource source);
    }
}
