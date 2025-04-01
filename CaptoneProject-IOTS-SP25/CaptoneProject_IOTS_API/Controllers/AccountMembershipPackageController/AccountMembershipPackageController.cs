﻿using CaptoneProject_IOTS_BOs.DTO.MembershipPackageDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.AccountMembershipPackageController
{
    [Route("api/account-membership-package")]
    [ApiController]
    public class AccountMembershipPackageController : MyBaseController.MyBaseController
    {
        private readonly IAccountMembershipPackageService accountMembershipPackageService;
        private readonly IActivityLogService activityLogService;
        public AccountMembershipPackageController(IAccountMembershipPackageService accountMembershipPackageService, IActivityLogService activityLogService)
        {
            this.accountMembershipPackageService = accountMembershipPackageService;
            this.activityLogService = activityLogService;
        }

        [HttpGet("get-all-membership-package-options")]
        public async Task<IActionResult> GetAllMembershipPackageOptions()
        {
            var res = await accountMembershipPackageService.GetAllMembershipPackage();
            return GetActionResult(res);
        }

        [HttpGet("get-account-membership-package/{userId}")]
        public async Task<IActionResult> GetAccountMembershipPackageByUserId(int userId)
        {
            var res = await accountMembershipPackageService.GetAccountMembershipPackageByUserId(userId);

            return GetActionResult(res);
        }

        [HttpPost("register-membership-package")]
        public async Task<IActionResult> RegisterMembershipPackage([FromBody] AccountRegisterMembershipPackageDTO payload)
        {
            var res = await accountMembershipPackageService.RegisterAccountMembershipPackage(payload);
            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Register to access website");
            }
            return GetActionResult(res);
        }
    }
}
