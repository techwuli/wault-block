using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaultBlock.Data;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public class WaultIdentityService : IWaultIdentityService, IDisposable
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

        public async Task<WaultWallet> CreateWalletAsync(string name, string userId)
        {
            Wallet indyWallet = null;
            try
            {

                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), name, InDbWalletType.WAULT_TYPE, null, "{\"userId\":\"" + userId + "\"}");
                indyWallet = await Wallet.OpenWalletAsync(name, null, "{\"userId\":\"" + userId + "\"}");
                var result = await Did.CreateAndStoreMyDidAsync(indyWallet, "{}");
                var wallet = await _dbContext.WaultWallets.FirstOrDefaultAsync(p => p.Name == name && p.UserId == userId);
                return wallet;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await indyWallet.CloseAsync();
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

        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;
                StorageUtils.CleanupStorage();
                Console.WriteLine("=== WaultIdentityService destroyed ===");
            }
        }

        public async Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null, bool? published = null)
        {
            var query = _dbContext.ClaimDefinitions
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.UserId == userId);
            }

            if (published.HasValue)
            {
                query = query.Where(p => p.Published == published);
            }

            return await query.ToListAsync();
        }

        public async Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, string name, string fields)
        {
            var claimDefinition = new ClaimDefinition
            {
                Id = Guid.NewGuid(),
                Name = name,
                Fields = fields,
                Published = false,
                UserId = userId
            };

            _dbContext.ClaimDefinitions.Add(claimDefinition);
            await _dbContext.SaveChangesAsync();
            return claimDefinition;
        }

        public async Task PublishClaimDefinitionAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
