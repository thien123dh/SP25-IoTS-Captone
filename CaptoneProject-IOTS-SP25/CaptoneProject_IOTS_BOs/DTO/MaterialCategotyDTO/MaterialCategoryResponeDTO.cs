using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO
{
    public class MaterialCategoryResponeDTO
    {
        public int Id { get; set; }
        public string Label { get; set; }

        [MaxLength(1000)]
        public string? Description { set; get; }

        public int? CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
        public int? Orders { get; set; }
        public int? IsActive { get; set; }
    }
}
