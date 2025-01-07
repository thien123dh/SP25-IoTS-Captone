using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class UserRequestResponseDTO
    {
        public int Id { set; get; }
        public string Email { set; get; }
        public UserRequestStatus? StatusNavigation { set; get; }
    }
}
