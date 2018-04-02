using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaultBlock.Identities.DataObjects;
using WaultBlock.Identities.Utils;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService
    {
        public async Task CreateTrustAnchorWalletAsync(ApplicationUser user)
        {
            // Get steward wallet
            var stewardWalletData = await GetStewardWalletDataAsync();
            var stewardWallet = await Wallet.OpenWalletAsync(stewardWalletData.Name, null, null);
            Wallet trustAnchorWallet = null;
            try
            {
                // steward create new DID
                var stewardTrustAnchorDidResult = await Did.CreateAndStoreMyDidAsync(stewardWallet, "{}");

                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {
                    // steward send NYM transaction
                    var nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, stewardTrustAnchorDidResult.Did, stewardTrustAnchorDidResult.VerKey, null, "TRUST_ANCHOR");
                    await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);

                    // steward creates the connection request
                    var nonce = RandomUtils.RandomNumber(8);
                    var connectionRequest = new { did = stewardTrustAnchorDidResult.Did, nonce };

                    // trust anchor create wallet
                    var trustAnchorWalletName = Guid.NewGuid().ToString();
                    await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), trustAnchorWalletName, null, null, null);
                    trustAnchorWallet = await Wallet.OpenWalletAsync(trustAnchorWalletName, null, null);
                    var trustAnchorStewardWalletDidResult = await Did.CreateAndStoreMyDidAsync(trustAnchorWallet, "{}");

                    // trust anchor create connection response
                    var connectionResponse = new { did = trustAnchorStewardWalletDidResult.Did, verKey = trustAnchorStewardWalletDidResult.VerKey, connectionRequest.nonce };

                    // trust anchor asks the ledger for the verkey of the Steward's DID
                    var stewardTrustAnchorVerKey = await Did.KeyForDidAsync(pool, trustAnchorWallet, connectionRequest.did);

                    // trust anchor encrypts the connection response
                    var anoncryptedConnectionResponse = await Crypto.AnonCryptAsync(stewardTrustAnchorVerKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(connectionResponse)));

                    // steward decrypts the connection response
                    var decryptedConnectionResponse = await Crypto.AnonDecryptAsync(stewardWallet, stewardTrustAnchorDidResult.VerKey, anoncryptedConnectionResponse);
                    var decryptedConnectionResponseObj = JObject.Parse(Encoding.UTF8.GetString(decryptedConnectionResponse));
                    if (decryptedConnectionResponseObj.Value<int>("nonce") != nonce)
                    {
                        throw new Exception("Decrypt connection response failed, nonce not match");
                    }

                    // steward sends NYM transaction for Trust Anchor's DID to the ledger
                    nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, decryptedConnectionResponseObj.Value<string>("did"), decryptedConnectionResponseObj.Value<string>("verKey"), null, "TRUST_ANCHOR");
                    await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);

                    // Getting verinym
                    // trust anchor creates new DID
                    var trustAnchorDidResult = await Did.CreateAndStoreMyDidAsync(trustAnchorWallet, "{}");
                    var authcryptedTrustAnchorDidInfoJson = await Crypto.AuthCryptAsync(trustAnchorWallet, trustAnchorStewardWalletDidResult.VerKey, stewardTrustAnchorVerKey, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                    {
                        did = trustAnchorDidResult.Did,
                        verkey = trustAnchorDidResult.VerKey
                    })));

                    // steward decrypts the received message
                    var authDecryptResult = await Crypto.AuthDecryptAsync(stewardWallet, stewardTrustAnchorVerKey, authcryptedTrustAnchorDidInfoJson);
                    var authDecryptedDidInfo = JObject.Parse(Encoding.UTF8.GetString(authDecryptResult.MessageData));

                    // steward  asks the ledger for the verkey of trust anchor's DID
                    var trustAnchorVerKey = await Did.KeyForDidAsync(pool, stewardWallet, authDecryptedDidInfo.Value<string>("did"));
                    if (trustAnchorVerKey != authDecryptResult.TheirVk)
                    {
                        throw new Exception("Decrypt auth failed, VerKey not match");
                    }

                    // steward send NYM transaction for Trust Anchor's DID
                    nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, authDecryptedDidInfo.Value<string>("did"), authDecryptedDidInfo.Value<string>("verkey"), null, "TRUST_ANCHOR");
                    await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);

                    await SaveWalletData(trustAnchorWalletName, user.Id, trustAnchorDidResult.Did, trustAnchorDidResult.VerKey, true);

                    await pool.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await stewardWallet.CloseAsync();
                await trustAnchorWallet.CloseAsync();
            }
        }

        public async Task CreateWalletAsync(ApplicationUser user, bool isTrustAnchor, string stewardId = null, string seed = null)
        {
            WalletData stewardWalletData = null;
            Wallet stewardWallet = null;

            if (!string.IsNullOrEmpty(stewardId))
            {
                stewardWalletData = await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.UserId == stewardId);
                if (stewardWalletData == null)
                {
                    throw new Exception("Agent Wallet not found.");
                }

                stewardWallet = await Wallet.OpenWalletAsync(stewardWalletData.Name, null, null);
            }

            var walletName = $"{user.Id}";
            Wallet wallet = null;
            try
            {
                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), walletName, null, null, null);
                wallet = await Wallet.OpenWalletAsync(walletName, null, null);

                var didJson = "{}";
                if (!string.IsNullOrEmpty(seed))
                {
                    didJson = DidJson.GetJson(seed);
                }

                var didResult = await Did.CreateAndStoreMyDidAsync(wallet, didJson);
                var walletData = new WalletData
                {
                    UserId = user.Id,
                    TimeCreated = DateTime.UtcNow,
                    Did = didResult.Did,
                    Name = walletName,
                    VerKey = didResult.VerKey
                };

                _dbContext.WalletDatas.Add(walletData);

                if (stewardWalletData != null)
                {
                    if (isTrustAnchor)
                    {
                        var stewardAgentDidResult = await Did.CreateAndStoreMyDidAsync(stewardWallet, "{}");
                        var nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, stewardAgentDidResult.Did, stewardAgentDidResult.VerKey, null, "TRUST_ANCHOR");

                        using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                        {
                            var res = await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);

                            var stewardAgentVerKey = await Did.KeyForDidAsync(pool, wallet, stewardAgentDidResult.Did);
                            var connectionResponse = JsonConvert.SerializeObject(new { did = stewardAgentDidResult.Did, verkey = stewardAgentVerKey, nonce = 21314123141 });
                            var anoncryptedConnectionResponse = await Crypto.AnonCryptAsync(stewardAgentVerKey, Encoding.UTF8.GetBytes(connectionResponse));
                            var decryptedConnectionResponse = Encoding.UTF8.GetString(await Crypto.AnonDecryptAsync(stewardWallet, stewardAgentDidResult.VerKey, anoncryptedConnectionResponse));
                            var connectionResponseObj = JObject.Parse(decryptedConnectionResponse);
                            nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, connectionResponseObj.Value<string>("did"), connectionResponseObj.Value<string>("verkey"), null, "TRUST_ANCHOR");
                            var signAndSubmitRequestResult = await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);

                            // getting verinym
                        }
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
                await CloseWalletAsync(wallet);
                await CloseWalletAsync(stewardWallet);
            }
        }

        public async Task CreateWalletForBusinessAsync(ApplicationUser user)
        {
            // Get steward wallet
            var stewardWalletData = await GetStewardWalletDataAsync();
            var stewardWallet = await Wallet.OpenWalletAsync(stewardWalletData.Name, null, null);
            Wallet trustAnchorWallet = null;
            try
            {
                // steward create new DID
                var stewardTrustAnchorDidResult = await Did.CreateAndStoreMyDidAsync(stewardWallet, "{}");

                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {
                    // steward send NYM transaction
                    var nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, stewardTrustAnchorDidResult.Did, stewardTrustAnchorDidResult.VerKey, null, "TRUST_ANCHOR");
                    var a = await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);
                    Console.WriteLine($"NYM 1: {a}");

                    // trust anchor create wallet
                    var trustAnchorWalletName = Guid.NewGuid().ToString();
                    await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), trustAnchorWalletName, null, null, null);
                    trustAnchorWallet = await Wallet.OpenWalletAsync(trustAnchorWalletName, null, null);
                    var trustAnchorStewardWalletDidResult = await Did.CreateAndStoreMyDidAsync(trustAnchorWallet, "{}");

                    // steward sends NYM transaction for Trust Anchor's DID to the ledger
                    nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, trustAnchorStewardWalletDidResult.Did, trustAnchorStewardWalletDidResult.VerKey, null, "TRUST_ANCHOR");
                    var b = await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);
                    Console.WriteLine($"NYM 2: {b}");

                    // Getting verinym
                    // trust anchor creates new DID
                    var trustAnchorDidResult = await Did.CreateAndStoreMyDidAsync(trustAnchorWallet, "{}");

                    // steward send NYM transaction for Trust Anchor's DID
                    nymRequest = await Ledger.BuildNymRequestAsync(stewardWalletData.Did, trustAnchorDidResult.Did, trustAnchorDidResult.VerKey, null, "TRUST_ANCHOR");
                    var c = await Ledger.SignAndSubmitRequestAsync(pool, stewardWallet, stewardWalletData.Did, nymRequest);
                    Console.WriteLine($"NYM 3: {c}");

                    await SaveWalletData(trustAnchorWalletName, user.Id, trustAnchorDidResult.Did, trustAnchorDidResult.VerKey, true);

                    await pool.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await stewardWallet.CloseAsync();
                await trustAnchorWallet.CloseAsync();
            }
        }

        public async Task CreateWalletForUserAsync(ApplicationUser user, string seed = null, bool isDefault = false)
        {
            var walletName = Guid.NewGuid().ToString();
            Wallet wallet = null;
            try
            {
                await Wallet.CreateWalletAsync(_configuration.GetValue("Indy.PoolName", "indy_pool"), walletName, null, null, null);
                wallet = await Wallet.OpenWalletAsync(walletName, null, null);

                var seedString = string.IsNullOrEmpty(seed) ? "{}" : DidJson.GetJson(seed);
                var didResult = await Did.CreateAndStoreMyDidAsync(wallet, seedString);
                await SaveWalletData(walletName, user.Id, didResult.Did, didResult.VerKey, true);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await CloseWalletAsync(wallet);
            }
        }

        public async Task<IEnumerable<WalletData>> GetWalletDatasAsync(string userId)
        {
            var result = await _dbContext.WalletDatas.Where(p => p.UserId == userId).ToListAsync();
            return result;
        }

        private async Task CloseWalletAsync(Wallet wallet)
        {
            if (wallet != null)
            {
                await wallet.CloseAsync();
            }
        }

        private async Task<WalletData> GetDefaultWalletDataAsync(string userId)
        {
            var walletData = await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.UserId == userId && p.IsDefault);
            if (walletData == null)
            {
                throw new Exception("Default wallet not found.");
            }

            return walletData;
        }

        private async Task<WalletData> GetStewardWalletDataAsync()
        {
            var stewardUser = (await _userManager.GetUsersInRoleAsync("steward")).FirstOrDefault();
            if (stewardUser == null)
            {
                throw new Exception("Steward not found.");
            }

            var stewardWalletData = await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.IsDefault && p.UserId == stewardUser.Id);
            return stewardWalletData;
        }

        private async Task<WalletData> GetWalletDataAsync(string walletName, string userId)
        {
            return await _dbContext.WalletDatas.FirstOrDefaultAsync(p => p.Name == walletName && p.UserId == userId);
        }

        private async Task SaveWalletData(string name, string userId, string did, string verKey, bool isDefault = false)
        {
            var walletData = new WalletData
            {
                UserId = userId,
                Did = did,
                VerKey = verKey,
                IsDefault = isDefault,
                Name = name,
                TimeCreated = DateTime.UtcNow
            };

            _dbContext.WalletDatas.Add(walletData);
            await _dbContext.SaveChangesAsync();
        }
    }
}
