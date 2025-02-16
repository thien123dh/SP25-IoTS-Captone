using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class SecondhandInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("iot_device_id")]
        [ForeignKey(nameof(IotsDevice))]
        public int IotDeviceId { set; get; }

        [Column("secondhand_price")]
        public decimal SecondHandPrice { set; get; } = 0;

        [Column("quality_percent")]
        [Range(0, 100)]
        public int QualityPercent { set; get; }
    }
}
