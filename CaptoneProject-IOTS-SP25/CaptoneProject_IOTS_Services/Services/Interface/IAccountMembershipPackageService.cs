using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MembershipPackageDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IAccountMembershipPackageService
    {
        public List<MembershipPackage> GetAllMembershipPackage();
        public MembershipPackage? GetMembershipPackageById(int id);
        public Task<GenericResponseDTO<AccountMembershipPackage>> CreateOrUpdateAccountMembershipPackage(CreateUpdateAccountMembershipPackage request);
        public Task<GenericResponseDTO<AccountMembershipPackage>> GetAccountMembershipPackageByUserId(int userId);
    }
}
