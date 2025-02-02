using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialDTO
{
    public class MaterialResponeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public int? FirmwareVersion { get; set; }

        public string Connectivity { get; set; }

        public string PowerSource { get; set; }

        public DateTime? LastWarrentyDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public int? IsActive { get; set; }
    }
}
