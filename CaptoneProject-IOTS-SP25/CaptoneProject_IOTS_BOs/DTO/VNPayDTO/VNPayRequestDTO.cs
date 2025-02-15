using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.VNPayDTO
{
    public class VNPayRequestDTO
    {
        [Url]
        public string urlResponse { get; set; } = null!;
    }
}
