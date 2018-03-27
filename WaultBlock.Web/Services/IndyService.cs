using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WaultBlock.Web.Data;
using WaultBlock.Web.Models;
using WaultBlock.Web.Models.ClaimsViewModels;
using Hyperledger.Indy.PoolApi;
using System;

namespace WaultBlock.Web.Services
{
    public class IndyService : IIndyService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndyService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;

            ConnectToIndyPool().Wait();
        }

        public async Task<List<IndyClaimViewModel>> GetClaimsAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            var result = new List<IndyClaimViewModel>();
            return result;
        }

        private async Task ConnectToIndyPool(){
            var pools = await Pool.ListPoolsAsync();
            Console.WriteLine(pools);
        }
    }
}
