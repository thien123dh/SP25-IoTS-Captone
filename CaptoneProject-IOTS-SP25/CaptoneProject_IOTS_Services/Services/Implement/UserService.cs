using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RoleDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using System.Net;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserService : IUserServices
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly ITokenServices _tokenGenerator;
        private readonly PasswordHasher<string> _passwordHasher;

        public UserService(
            UserRepository userService, 
            ITokenServices tokenGenerator,
            UserRoleRepository userRoleRepository
        )
        {
            _userRepository = userService;
            _userRoleRepository = userRoleRepository;
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
                Data = (await GetUserDetailsById(u.Id)).Data
            };
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
                        return
                        UserMapper.mapToUserDetailResponse(item);
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

        public async Task<ResponseDTO> GetUserDetailsById(int id)
        {
            User user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var data = UserMapper.mapToUserDetailResponse(user);

            return
                new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "OK",
                    StatusCode = HttpStatusCode.OK,
                    //Data = _map.MappingTo(user)
                    Data = data
                };
        }

        public async Task<ResponseDTO> UpdateUserRole(int userId, List<int>? roleList)
        {
            roleList = (roleList == null) ? new List<int>() : roleList;
            User user = await _userRepository.GetUserById(userId);

            if (user == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest
                };

            List<UserRole> userRoleList = user.UserRoles.ToList();

            //Get the user role doesn't exist in input user role list
            IEnumerable<UserRole> deleteUserRoleList = userRoleList.Where(ur => !roleList.Contains(ur.RoleId));
            //REMOVE user role
            await _userRoleRepository.RemoveAsync(deleteUserRoleList);

            var insertUserRoleList = roleList
            .Where(
                r => (userRoleList.FirstOrDefault(ur => ur.RoleId == r) == null)
            )
            .Select(r => new UserRole
            {
                UserId = userId,
                RoleId = r,
                CreatedDate = DateTime.Now,
                //HARDCODE ==> TODO
                CreatedBy = 1
            });

            //Create user role list
            await _userRoleRepository.CreateAsync(insertUserRoleList);

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Ok",
                StatusCode = HttpStatusCode.OK,
                Data = (await GetUserDetailsById(userId)).Data
            };
        }

        public async Task<ResponseDTO> GetUsersPagination(PaginationRequest paginationRequest, int? roleId)
        {
            PaginationResponse<User> response = _userRepository.GetPaginate(
                    filter: user => (
                        ((roleId == null) || user.UserRoles.SingleOrDefault(userRole => userRole.RoleId == roleId) != null)
                        &&
                        (
                            user.Email.Contains(paginationRequest.searchKeyword)
                            ||
                            user.Username.Contains(paginationRequest.searchKeyword)
                            ||
                            user.Fullname.Contains(paginationRequest.searchKeyword)
                        )
                    ),
                    orderBy: null,
                    includeProperties: "UserRoles,UserRoles.Role",
                    pageIndex: paginationRequest.PageIndex,
                    pageSize: paginationRequest.PageSize
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Ok",
                StatusCode = HttpStatusCode.OK,
                Data = PaginationMapper<User, UserDetailsResponseDTO>.mappingTo(UserMapper.mapToUserDetailResponse, source: response)
            };
        }
    }
}
