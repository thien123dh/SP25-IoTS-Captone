using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserService : IUserServices
    {
        private readonly UserRepository _userRepository;
        private readonly ITokenServices _tokenGenerator;
        private readonly PasswordHasher<string> _passwordHasher;

        public UserService(
            UserRepository userService, 
            ITokenServices tokenGenerator
        )
        {
            _userRepository = userService;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = new PasswordHasher<string>();
        }

        public async Task<ResponseDTO> UpdateUserStatus(int userId, int isActive)
        {
            User u = _userRepository.GetById(userId);

            if (u == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            u.IsActive = isActive;
            _userRepository.Update(u);

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "OK",
                StatusCode = HttpStatusCode.OK,
                Data = new
                {
                    Id = u.Id,
                }
            };
        }

        public Task<ResponseDTO> DeactiveUser(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO> GetAllUsers()
        {
            List<User> userList = _userRepository.GetAll();

            if (userList == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "User List is empty",
                    StatusCode = HttpStatusCode.Unauthorized,
                };
            }

            return new ResponseDTO
            {
                IsSuccess = true,
                Data = userList.Select(
                    item =>
                    {
                        return new
                        {
                            id = item.Id,
                            username = item.Username,
                            email = item.Email,
                            address = item.Address,
                            role = item.UserRoles?.FirstOrDefault()?.Role.Label,
                            isActive = item.IsActive == 1
                        };
                    }
                )
            };
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
                    Role = user.UserRoles?.FirstOrDefault()?.Role.Label,
                    user.Email,
                    user.Address,
                    user.Phone,
                    user.IsActive,
                }
            };
        }

        public Task<ResponseDTO> UpdateUserRole(int userId, int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
