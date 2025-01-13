using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class ChangePasswordRequestDTO
    {
        [Required]
        public string OldPassword { set; get; }
        [Required]
        public string NewPassword { set; get; }
    }
}
