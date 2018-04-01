using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WaultBlock.Identities.Utils;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService
    {
        public async Task CreateWalletAsync(ApplicationUser user, string agentId = null)
        {
            WalletData agentWalletData = null;
            Wallet agentWallet = null;

            if (!string.IsNullOrEmpty(agentId))
            {
                agentWalletData = await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.UserId == agentId);
                if (agentWalletData == null)
                {
                    throw new Exception("Agent Wallet not found.");
                }

                agentWallet = await Wallet.OpenWalletAsync(agentWalletData.Name, null, null);
            }

            var walletName = $"{user.Id}";
            Wallet wallet = null;
            try
            {
                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), walletName, null, null, null);
                wallet = await Wallet.OpenWalletAsync(walletName, null, null);
                var result = await Did.CreateAndStoreMyDidAsync(wallet, "{}");
                var walletData = new WalletData
                {
                    UserId = user.Id,
                    TimeCreated = DateTime.UtcNow,
                    Did = result.Did,
                    Name = walletName,
                    VerKey = result.VerKey
                };

                _dbContext.WalletDatas.Add(walletData);

                if (agentWalletData != null)
                {
                    var nymRequest = await Ledger.BuildNymRequestAsync(agentWalletData.Did, result.Did, agentWalletData.VerKey, null, null);

                    using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                    {
                        var res = await Ledger.SignAndSubmitRequestAsync(pool, agentWallet, agentWalletData.Did, nymRequest);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await CloseWallet(wallet);
                await CloseWallet(agentWallet);
            }
        }

        public async Task<IEnumerable<WalletData>> GetWalletDatasAsync(string userId)
        {
            var result = await _dbContext.WalletDatas.Where(p => p.UserId == userId).ToListAsync();
            return result;
        }



        private async Task CloseWallet(Wallet wallet)
        {
            if (wallet != null)
            {
                await wallet.CloseAsync();
            }
        }

        private async Task<WalletData> GetWalletDataAsync(string walletName, string userId)
        {
            return await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.Name == walletName && p.UserId == userId);
        }
    }
}
