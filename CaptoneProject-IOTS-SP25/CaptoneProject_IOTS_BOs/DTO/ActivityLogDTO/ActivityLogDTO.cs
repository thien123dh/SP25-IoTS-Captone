using CaptoneProject_IOTS_BOs.Models;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO
{
    public class CreateActivityLogDTO
    {
        public int EntityId {set; get;}
        public int EntityType { set; get; }
        public string Title { set; get; }
        public string Contents { set; get; }
        public string? MetaData { set; get; }

    }

    public class EntityTypeDTO
    {
        public int Id { set; get; }
        public string? label { set; get; }
        public int IsActive { set; get; }
    }

    public class ActivityLogResponseDTO : ActivityLog
    {
        public string EntityTypeLabel { set; get; }
    }
}
