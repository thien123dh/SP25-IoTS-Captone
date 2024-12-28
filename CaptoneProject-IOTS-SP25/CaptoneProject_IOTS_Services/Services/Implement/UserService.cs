using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Repository.Repository.Interface;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserService : IUserServices
    {
        private readonly UserRepository _userRepository;
        private readonly ITokenServices _tokenGenerator;
        private readonly PasswordHasher<string> _passwordHasher;

        public UserService(UserRepository userService, ITokenServices tokenGenerator)
        {
            _userRepository = userService;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = new PasswordHasher<string>();
        }
        public async Task<ResponseDTO> LoginUserAsync(string email, string password)
        {
            var user = await _userRepository.CheckLoginAsync(email, password); //Pass default: 123
            if (user != null)
            {
                var verifyPassword = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
                if (verifyPassword == PasswordVerificationResult.Failed)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid credentials",
                        StatusCode = HttpStatusCode.Unauthorized,
                    }; // Invalid credentials
                }
                else if (user.IsActive != 1)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Account not allow",
                        StatusCode = HttpStatusCode.Unauthorized,
                    };
                }
            }
            else
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "User not found",
                    StatusCode = HttpStatusCode.NotFound,
                }; // Invalid credentials
            }
            var token = _tokenGenerator.GenerateToken(user);
            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Login success",
                StatusCode = HttpStatusCode.OK,
                Data = new
                {
                    Token = token,
                    user.Id,
                    user.Username,
                    Role = user.UserRoles.FirstOrDefault().Role.Label,
                    user.Email,
                    user.Address,
                    user.Phone,
                    user.IsActive,
                }
            };
        }
    }
}
