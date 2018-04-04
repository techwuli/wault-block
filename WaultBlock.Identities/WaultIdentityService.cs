using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using WaultBlock.Data;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService : IWaultIdentityService, IDisposable
    {
        private IConfiguration _configuration;
        private ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        private bool _disposing;

        public WaultIdentityService(ApplicationDbContext dbContext, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _userManager = userManager;
            // StorageUtils.CleanupStorage();
            Console.WriteLine("=== WaultIdentityService started ===");
        }

        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;
                // StorageUtils.CleanupStorage();
                Console.WriteLine("=== WaultIdentityService destroyed ===");
            }
        }
    }
}
