using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.RoleDTO
{
    public class RoleResponse
    {
        public int Id { set; get; }
        public string Label { set; get; }
        public int? Orders { set; get; }
        public int? IsActive { set; get; }
    }
}
