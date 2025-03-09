using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class DeviceSpecificationsItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public string? SpecificationProperty { set; get; }

        public string? SpecificationValue { set; get; }

        [ForeignKey(nameof(DeviceSpecification))]
        public int DeviceSpecificationId { set; get; }

        public virtual DeviceSpecification DeviceSpecification { set; get; }
        public DateTime CreatedDate { set; get; }
    }
}
