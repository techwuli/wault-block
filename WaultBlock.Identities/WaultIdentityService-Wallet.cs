using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WaultBlock.Data;
using WaultBlock.Identities.DataObjects;
using WaultBlock.Identities.Utils;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService
    {
        public static void RegisterWalletType(ApplicationDbContext dbContext)
        {
            Console.WriteLine("Register Wallet Type");
            Wallet.RegisterWalletTypeAsync(InDbWalletType.WAULT_TYPE, new InDbWalletType(dbContext)).Wait();
        }

        public async Task<WalletData> CreateWalletAsync(ApplicationUser user, string agentId = null)
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

                agentWallet = await OpenWalletAsync(agentWalletData.Name, null, "{\"userId\":\"" + agentId + "\"}");
            }

            var walletName = $"{user.UserName}_default_wallet";
            Wallet wallet = null;
            WalletData walletData = null;
            try
            {
                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), walletName, InDbWalletType.WAULT_TYPE, null, "{\"userId\":\"" + user.Id + "\"}");
                wallet = await Wallet.OpenWalletAsync(walletName, null, "{\"userId\":\"" + user.Id + "\"}");
                var result = await Did.CreateAndStoreMyDidAsync(wallet, "{}");

                if (agentWalletData != null)
                {
                    var agentWalletKeys = await GetWalletKeysAsync(agentWalletData.Name, agentId);
                    var nymRequest = await Ledger.BuildNymRequestAsync(agentWalletKeys.Did, result.Did, result.VerKey, null, null);

                    using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                    {
                        var res = await Ledger.SignAndSubmitRequestAsync(pool, agentWallet, agentWalletKeys.Did, nymRequest);
                    }
                }

                //  await _dbContext.SaveChangesAsync();
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

            return walletData;
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

        private async Task<Wallet> OpenWalletAsync(string name, string config, string credentials)
        {
            var tempPath = Path.Combine(EnvironmentUtils.GetIndyHomePath(), "wallet", name);
            Directory.CreateDirectory(tempPath);
            using (var fileStream = File.Create(Path.Combine(tempPath, "wallet.json")))
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteAsync("{\"pool_name\":\"indy_pool\",\"xtype\":\"indb\",\"name\":\"" + name + "\"}");
            }
            return await Wallet.OpenWalletAsync(name, config, credentials);
        }

        public async Task<WalletKeys> GetWalletKeysAsync(string walletName, string userId)
        {
            var records = await _dbContext.WalletRecords.Where(p => p.WalletName == walletName && p.UserId == userId).ToListAsync();
            WalletKeys result = null;
            if (records.Any())
            {
                result = new WalletKeys();
                var didRecord = records.FirstOrDefault(p => p.Key.StartsWith("my_did"));
                var didRecordValue = JObject.Parse(didRecord.Value);
                result.Did = didRecordValue.Value<string>("did");
                result.Key = didRecordValue.Value<string>("verkey");
                var keyRecord = records.FirstOrDefault(p => p.Key.StartsWith("key"));
                var keyRecordValue = JObject.Parse(keyRecord.Value);
                result.SignKey = keyRecordValue.Value<string>("signkey");
            }
            return result;
        }
    }
}
