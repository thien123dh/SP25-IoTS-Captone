using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO
{
    public class MaterialCategoryResponeDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int? Orders { get; set; }
        public int? IsActive { get; set; }
    }
}
