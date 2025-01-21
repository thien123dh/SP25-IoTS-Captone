using Microsoft.EntityFrameworkCore.Storage;
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
        [EmailAddress]
        public string Email { get; set; }
        public int UserRequestStatus { get; set; }
        public int RoleId { set; get; }
        public string? Remark { set; get; }
    }

    public class CreateUserRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class RemarkDTO
    {
        [Required]
        public string Remark { set; get; }
    }
}
