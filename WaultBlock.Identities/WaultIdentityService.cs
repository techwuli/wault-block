using System;
using Microsoft.Extensions.Configuration;
using WaultBlock.Data;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService : IWaultIdentityService, IDisposable
    {
        private IConfiguration _configuration;
        private ApplicationDbContext _dbContext;

        private bool _disposing;

        public WaultIdentityService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            StorageUtils.CleanupStorage();
            Console.WriteLine("=== WaultIdentityService started ===");
        }

        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;
                StorageUtils.CleanupStorage();
                Console.WriteLine("=== WaultIdentityService destroyed ===");
            }
        }
    }
}
