using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class IotsDevicesCombo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(Combo))]
        public int ComboId { set; get; }

        [ForeignKey(nameof(IotsDevice))]
        public int IotDeviceId { set; get; }

        [ForeignKey(nameof(IotDeviceId))]
        public virtual IotsDevice? IotDeviceNavigation { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.Now;
    }
}
