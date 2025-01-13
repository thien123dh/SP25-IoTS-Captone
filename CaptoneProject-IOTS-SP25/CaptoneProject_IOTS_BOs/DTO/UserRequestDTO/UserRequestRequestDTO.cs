using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserRequestDTO
{
    public class UserRequestRequestDTO
    {
        [Required]
        public string Email { get; set; }
        public int UserRequestStatus { get; set; }
    }

    public class CreateUserRequestDTO
    {
        [Required]
        public string Email { get; set; }
    }
}
