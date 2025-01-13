﻿using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserRequestDTO
{
    public class UserRequestStatusDTO
    {
        public int Id { set; get; }
        public string Label { set; get; }
    }
    public class UserRequestResponseDTO
    {
        public int Id { set; get; }
        public string Email { set; get; }

        //public UserDetailsResponseDTO? UserDetails { set; get; }
        public UserRequestStatusDTO? UserRequestStatus { set; get; }
        public string? Remark { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? ActionDate { set; get; }
    }
}