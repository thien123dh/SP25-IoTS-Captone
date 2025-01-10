using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RoleDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
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
        private readonly IUserRequestService userRequestService;
        private readonly UserRequestRepository userRequestRepository;

        public UserService(
            UserRepository userService, 
            ITokenServices tokenGenerator,
            UserRoleRepository userRoleRepository,
            IUserRequestService userRequestService,
            UserRequestRepository userRequestRepository
        )
        {
            _userRepository = userService;
            _userRoleRepository = userRoleRepository;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = new PasswordHasher<string>();
            this.userRequestService = userRequestService;
            this.userRequestRepository = userRequestRepository;
        }

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserLoginInfo(ClaimsPrincipal user)
        {
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new GenericResponseDTO<UserDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "User not authenticated",
                    StatusCode = HttpStatusCode.Unauthorized,
                    Data = null
                };
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            User userDb = await _userRepository.GetUserById(int.Parse(userId));

            return new GenericResponseDTO<UserDetailsResponseDTO>
            {
                IsSuccess = true,
                Message = "Role check success",
                StatusCode = HttpStatusCode.OK,
                Data = UserMapper.mapToUserDetailResponse(userDb)
            };
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

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id)
        {
            User user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return new GenericResponseDTO<UserDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_DOESNT_EXIST,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var data = UserMapper.mapToUserDetailResponse(user);

            return
                new GenericResponseDTO<UserDetailsResponseDTO>
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
                    Message = ExceptionMessage.USER_DOESNT_EXIST,
                    StatusCode = HttpStatusCode.NotFound
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

        public async Task<GenericResponseDTO<UserDetailsResponseDTO>> CreateOrUpdateUser(int id, UserCreateOrUpdateRequestDTO payload)
        {
            User user = await _userRepository.GetUserByEmail(payload.Email);

            if (user != null)
            {
                return new GenericResponseDTO<UserDetailsResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ExceptionMessage.USER_EXIST_EXCEPTION
                };
            }

            User? loginUser = (await GetLoginUser()).Data;

            user = new User
            {
                Email = payload.Email,
                Fullname = payload.Fullname,
                Username = payload.Email,
                Phone = payload.Phone,
                Address = payload.Address,
                Password = payload.Password == null ? "" : payload.Password,
                CreatedBy = loginUser?.Id,
                CreatedDate = DateTime.Now,
                UpdatedBy = loginUser?.Id,
                UpdatedDate = DateTime.Now
            };
            User newUser;
            try
            {
                newUser = _userRepository.Create(user);

                ResponseDTO response = await UpdateUserRole(newUser.Id, [payload.RoleId]);

                if (!response.IsSuccess)
                    throw new Exception(response.Message);
            }
            catch (Exception ex)
            {
                return new GenericResponseDTO<UserDetailsResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
            }

            return new GenericResponseDTO<UserDetailsResponseDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = (await GetUserDetailsById(newUser.Id))?.Data
            };
        }

        public async Task<GenericResponseDTO<User>> GetLoginUser()
        {
            return new GenericResponseDTO<User>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = new User
                {
                    Id = 1
                }
            };
        }

        public async Task<ResponseDTO> CreateStaffOrManager(UserCreateOrUpdateRequestDTO payload)
        {
            if (payload.RoleId != (int)RoleEnum.MANAGER && payload.RoleId != (int)RoleEnum.STAFF)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ExceptionMessage.INVALID_STAFF_MANAGER_ROLE
                };
            }

            GenericResponseDTO<UserDetailsResponseDTO> createUserResponse = await CreateOrUpdateUser(0, payload);

            if (!createUserResponse.IsSuccess)
                return createUserResponse;

            GenericResponseDTO<UserRequestResponseDTO> createRequestResponse = await userRequestService.CreateOrUpdateUserRequest(payload.Email, (int)UserRequestStatusEnum.PENDING_TO_VERIFY_OTP);

            return new ResponseDTO
            {
                IsSuccess = createRequestResponse.IsSuccess,
                StatusCode = createRequestResponse.StatusCode,
                Message = createRequestResponse.Message,
                Data = new
                {
                    requestId = createRequestResponse?.Data?.Id
                }
            };
        }

        public async Task<ResponseDTO> StaffManagerVerifyOTP(
            string otp, 
            int requestId, 
            int requestStatusId, 
            string password
        )
        {
            UserRequest userRequest = userRequestRepository.GetById(requestId);
            
            string email = userRequest.Email;

            if (userRequest == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND,
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            User user = await _userRepository.GetUserByEmail(userRequest.Email);

            ResponseDTO otpResponse = await userRequestService.VerifyOTP(email, otp);

            if (otpResponse.IsSuccess)
            {
                if (user != null)
                {
                    user.Password = _passwordHasher.HashPassword(null, password);
                    user.UpdatedDate = DateTime.Now;
                    user.IsActive = 1;
                    //Update user status to active
                    _userRepository.Update(user);
                }

                userRequest.Status = requestStatusId;
                UserRequest response = userRequestRepository.Update(userRequest);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    Data = new
                    {
                        RequestId = response.Id
                    }
                };
            }

            return otpResponse;
        }
    }
}
