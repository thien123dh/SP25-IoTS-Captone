using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MyHttpAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserRepository userRepository;
        public MyHttpAccessor(
            IHttpContextAccessor httpContextAccessor,
            UserRepository userRepository
        )
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
        }

        public int? GetLoginUserId()
        {
            ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;

            int? userId = null;

            try
            {
                userId = (user == null) ? null : int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {

            }

            return userId;
        }

        public User? GetLoginUser()
        {
            int? userId = GetLoginUserId();

            return (userId == null) ? null : userRepository.GetById((int)userId);
        }
    }
}
