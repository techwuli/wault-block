using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaultBlock.Data;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public class WaultIdentityService : IWaultIdentityService
    {
        private IConfiguration _configuration;
        private ApplicationDbContext _dbContext;

        public WaultIdentityService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            StorageUtils.CleanupStorage();
        }

        public async Task<WaultWallet> CreateWalletAsync(string name, string userId)
        {
            Wallet indyWallet = null;
            try
            {
                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), name, InDbWalletType.WAULT_TYPE, null, "{\"userId\":\"" + userId + "\"}");
                indyWallet = await Wallet.OpenWalletAsync(name, null, "{\"userId\":\"" + userId + "\"}");
                var result = await Did.CreateAndStoreMyDidAsync(indyWallet, "{\"seed\": \"000000000000000000000000Steward1\"}");

                var wallet = await _dbContext.WaultWallets.FirstOrDefaultAsync(p => p.Name == name && p.UserId == userId);
                return wallet;
            }
            catch (Exception ex)
            {
                if (indyWallet != null)
                {
                    await indyWallet.CloseAsync();
                }

                throw;
            }
        }

        public async Task<IEnumerable<WaultWallet>> GetWalletsAsync(string userId)
        {
            var result = await _dbContext.WaultWallets.Include(p => p.Records).Where(p => p.UserId == userId).ToListAsync();
            return result;
        }

        public static void RegisterWalletType(ApplicationDbContext dbContext)
        {
            Wallet.RegisterWalletTypeAsync(InDbWalletType.WAULT_TYPE, new InDbWalletType(dbContext)).Wait();
        }
    }
}
