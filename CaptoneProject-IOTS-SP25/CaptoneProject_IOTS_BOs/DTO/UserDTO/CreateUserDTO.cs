using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.GenderConst;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class ContactInformationDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^(?:\+84|0)(3[2-9]|5[2689]|7[0-9]|8[1-9]|9[0-9])[0-9]{7}$",
        ErrorMessage = "Invalid contact number")]
        public string? Phone { get; set; }
    }
    public class CreateUserDTO : ContactInformationDTO
    {
        [Required]
        public string? Fullname { set; get; }
        public string? Address { get; set; }
        public GenderEnum Gender { set; get; } = GenderEnum.Male;
        [Required]
        public int RoleId {get; set;}
    }

    public class UserRegisterDTO
    {
        [Required]
        public CreateUserDTO UserInfomation { get; set; }
        [Required]
        public string Otp { set; get; }
        [Required]
        public string Password { set; get; }
    }
}
