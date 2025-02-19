﻿using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserService : IUserServices
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly ITokenServices _tokenGenerator;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly MyHttpAccessor httpAccessor;
        private readonly WalletRepository walletRepository;
        public UserService (
            UserRepository userService, 
            ITokenServices tokenGenerator,
            UserRoleRepository userRoleRepository,
            MyHttpAccessor httpAccessor,
            WalletRepository walletRepository
        )
        {
            _userRepository = userService;
            _userRoleRepository = userRoleRepository;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = new PasswordHasher<string>();
            this.httpAccessor = httpAccessor;
            this.walletRepository = walletRepository;
        }

        public async Task<ResponseDTO> UpdateUserPassword(int userId, string password)
        {
            if (userId == 0)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User id is invalid"
                };
            }
            int? loginUserId = GetLoginUserId();

            User user = _userRepository.GetById(userId);
            user.Password = _passwordHasher.HashPassword(null, password);
            user.UpdatedBy = loginUserId;

            _userRepository.Update(user);

            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
            };

        }
        public async Task<GenericResponseDTO<UserResponseDTO>> GetUserLoginInfo()
        {
            int? loginUserId = GetLoginUserId();

            if (loginUserId == null)
                return 
                new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Not Login User",
                    StatusCode = HttpStatusCode.BadRequest
                };

            var user = await _userRepository.GetUserById((int)loginUserId);

            return
                new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = true,
                    Message = "Success",
                    StatusCode = HttpStatusCode.OK,
                    Data = UserMapper.mapToUserResponse(user)
                };

                
        }
        public async Task<GenericResponseDTO<UserResponseDTO>> UpdateUserStatus(int userId, int isActive)
        {
            User u = _userRepository.GetById(userId);

            if (u == null)
            {
                return new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = false,
                    Message = "User does not exist",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            u.IsActive = isActive;
            u = _userRepository.Update(u);

            return new GenericResponseDTO<UserResponseDTO>
            {
                IsSuccess = true,
                Message = "OK",
                StatusCode = HttpStatusCode.OK,
                Data = UserMapper.mapToUserResponse(u)
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
                        UserMapper.mapToUserResponse(item);
                    }
                )
            };
        }
        public async Task<ResponseDTO> LoginUserAsync(string email, string password)
        {
            var user = await _userRepository.CheckLoginAsync(email, password);

            if (user != null)
            {
                var verifyPassword = _passwordHasher.VerifyHashedPassword(null, user.Password, password);
                if (verifyPassword == PasswordVerificationResult.Failed)
                {
                    return ResponseService<Object>.BadRequest(ExceptionMessage.INCORRECT_PASSWORD);
                }
                else if (user.IsActive < 1)
                {
                    return ResponseService<Object>.Unauthorize(ExceptionMessage.LOGIN_INACTIVE_ACCOUNT);
                }
            }
            else
            {
                return ResponseService<Object>.NotFound(ExceptionMessage.EMAIL_DOESNT_EXIST);
            }
            try
            {
                var token = _tokenGenerator.GenerateToken(user);

                return ResponseService<Object>.OK(new
                {
                    Token = token,
                    user.Id,
                    user.Username,
                    Role = user.UserRoles?.FirstOrDefault()?.Role.Label,
                    RoleId = user.UserRoles?.FirstOrDefault()?.Role.Id,
                    user.Email,
                    user.Address,
                    user.Phone,
                    user.IsActive,
                });
            } catch (Exception ex)
            {
                return ResponseService<Object>.Unauthorize(ex.Message);
            }
            
        }

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id)
        {
            User user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return ResponseService<UserDetailsResponseDTO>.BadRequest(ExceptionMessage.USER_DOESNT_EXIST);
            }

            var data = UserMapper.mapToUserDetailsResponse(user);

            return ResponseService<UserDetailsResponseDTO>.OK(data);
        }

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> UpdateUserRole(int userId, List<int>? roleList)
        {
            roleList = (roleList == null) ? new List<int>() : roleList;
            User user = await _userRepository.GetUserById(userId);

            if (user == null)
                return ResponseService<UserDetailsResponseDTO>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            List<UserRole>? userRoleList = user.UserRoles?.ToList();

            //Get the user role doesn't exist in input user role list
            IEnumerable<UserRole>? deleteUserRoleList = userRoleList?.Where(ur => !roleList.Contains(ur.RoleId));
            //REMOVE user role
            if (deleteUserRoleList != null)
                await _userRoleRepository.RemoveAsync(deleteUserRoleList);

            int? loginUserId = GetLoginUserId();

            var insertUserRoleList = roleList
            .Where(
                r => (userRoleList?.FirstOrDefault(ur => ur.RoleId == r) == null)
            )
            .Select(r => new UserRole
            {
                UserId = userId,
                RoleId = r,
                CreatedDate = DateTime.Now,
                //HARDCODE ==> TODO
                CreatedBy = loginUserId == null ? 0 : (int)loginUserId
            });

            //Create user role list
            await _userRoleRepository.CreateAsync(insertUserRoleList);

            return ResponseService<UserDetailsResponseDTO>.OK((await GetUserDetailsById(userId)).Data);
        }

        public async Task<ResponseDTO> GetUsersPagination(PaginationRequest paginationRequest, int? roleId)
        {
            PaginationResponseDTO<User> response = _userRepository.GetPaginate(
                    filter: user => (
                    !user.UserRoles.Any(userRole => userRole.RoleId == 1) &&
                    ((roleId == null) || user.UserRoles.SingleOrDefault(userRole => userRole.RoleId == roleId) != null)
                        &&
                        (
                            user.Email.Contains(paginationRequest.SearchKeyword)
                            ||
                            user.Username.Contains(paginationRequest.SearchKeyword)
                            ||
                            user.Fullname.Contains(paginationRequest.SearchKeyword)
                        )
                    ),
                    orderBy: orderBy => orderBy.OrderByDescending(u => u.CreatedDate),
                    includeProperties: "UserRoles,UserRoles.Role",
                    pageIndex: paginationRequest.PageIndex,
                    pageSize: paginationRequest.PageSize
            );

            return ResponseService<Object>
                .OK(PaginationMapper<User, UserResponseDTO>.MappingTo(UserMapper.mapToUserResponse, source: response));
        }
        
        //set id = 0 to create new
        public async Task<GenericResponseDTO<UserResponseDTO>> CreateOrUpdateUser(int id, CreateUserDTO payload, int isActive)
        {
            User user = await _userRepository.GetUserByEmail(payload.Email);

            if (user != null)
            {
                return ResponseService<UserResponseDTO>.BadRequest(ExceptionMessage.USER_EXIST_EXCEPTION);
            }

            int? loginUserId = GetLoginUserId();

            user = user == null ? new User() : user;

            //Set Data
            user.Email = payload.Email;
            user.Fullname = payload.Fullname;
            user.Username = payload.Email;
            user.Phone = payload.Phone;
            user.Address = payload.Address;
            user.UpdatedDate = DateTime.Now;
            user.IsActive = isActive;
            user.UpdatedBy = loginUserId;
            user.UpdatedDate = DateTime.Now;
            user.Gender = (int)payload.Gender;
            //Set Data

            try
            {
                if (user.Id > 0) //Update
                    user = _userRepository.Update(user);
                else //Create
                {
                    user = _userRepository.Create(user);

                    //Create Default Wallet
                    walletRepository.Create(new Wallet
                    {
                        Ballance = 0,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        UserId = user.Id
                    });
                }

                ResponseDTO response = await UpdateUserRole(user.Id, [payload.RoleId]);

                if (!response.IsSuccess)
                    throw new Exception(response.Message);
            }
            catch (Exception ex)
            {
                return ResponseService<UserResponseDTO>.BadRequest(ex.Message);
            }

            return ResponseService<UserResponseDTO>.OK(UserMapper.mapToUserResponse(user));
        }

        //TODO GET LOGIN USER
        public int? GetLoginUserId()
        {
            int? loginUserId = httpAccessor.GetLoginUserId();

            return loginUserId;
        }
        public async Task<ResponseDTO> UserChangePassword(ChangePasswordRequestDTO payload)
        {
            User? loginUser = GetLoginUser();

            if (loginUser == null)
                return ResponseService<Object>.Unauthorize(ExceptionMessage.SESSION_EXPIRED);

            try
            {
                var verifyPassword = _passwordHasher.VerifyHashedPassword(null, loginUser.Password, payload.OldPassword);

                if (verifyPassword == PasswordVerificationResult.Failed)
                    return ResponseService<Object>.BadRequest("Incorrect currently password. Please try again");
                else
                {
                    return await UpdateUserPassword(loginUser.Id, payload.NewPassword);
                }

            }
            catch (Exception ex)
            {
                return ResponseService<Object>.BadRequest(ex.Message);
            }
        }

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsByEmail(string email)
        {
            User user = await _userRepository.GetUserByEmail(email);

            if (user == null)
                return ResponseService<UserDetailsResponseDTO>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            return await GetUserDetailsById(user.Id);
        }

        public User GetLoginUser()
        {
            int? loginUserId = GetLoginUserId();

            if (loginUserId == null)
                return null;

            User user = _userRepository.GetById((int)loginUserId);

            return user;
        }

        public async Task<List<Role>?> GetLoginUserRoles()
        {
            int? loginUserId = GetLoginUserId();

            if (loginUserId == null)
                return null;

            var user = await _userRepository.GetUserById((int)loginUserId);

            return user?.UserRoles?.Select(u => u.Role)?.ToList();
        }

        public async Task<Boolean> CheckLoginUserRole(RoleEnum role)
        {
            var roles = await GetLoginUserRoles();

            return roles?.Count(item => item.Id == (int)role) > 0;
        }

        public async Task<bool> CheckUserRole(int userId, RoleEnum role)
        {
            var user = await GetUserDetailsById(userId);

            if (user == null)
                return false;

            return user?.Data?.Roles?.Count(item => item.Id == (int)role) > 0;
        }
    }
}
