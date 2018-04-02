using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaultBlock.Identities.Utils;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService
    {
        public async Task CreateCredentialSchemaAsync(string userId, string name, string version, string[] attributes)
        {
            var walletData = await GetDefaultWalletDataAsync(userId);
            var schema = new { name, version, attr_names = attributes };
            var wallet = await Wallet.OpenWalletAsync(walletData.Name, null, null);
            try
            {
                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {
                    var schemaRequest = await Ledger.BuildSchemaRequestAsync(walletData.Did, JsonConvert.SerializeObject(schema));
                    var result = await Ledger.SignAndSubmitRequestAsync(pool, wallet, walletData.Did, schemaRequest);

                    Console.WriteLine("=============Schema Request Result===========");
                    Console.WriteLine(result);

                    var schemaData = new CredentialSchema
                    {
                        Id = Guid.NewGuid(),
                        AttributeArray = attributes,
                        Name = name,
                        Version = version,
                        UserId = userId
                    };

                    _dbContext.CredentialSchemas.Add(schemaData);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await wallet.CloseAsync();
            }
        }

        private async Task<JObject> GetSchemaAsync(Pool pool, string requesterDid, string creatorWalletDid, string schemaName, string schemaVersion)
        {
            var schemaData = JsonConvert.SerializeObject(new { name = schemaName, version = schemaVersion });
            var getSchemaRequest = await Ledger.BuildGetSchemaRequestAsync(requesterDid, creatorWalletDid, schemaData);
            var getSchemaResponse = await Ledger.SubmitRequestAsync(pool, getSchemaRequest);
            return JObject.Parse(getSchemaResponse);
        }

        public async Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, Guid credentialSchemaId)
        {
            var schema = await _dbContext.CredentialSchemas.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == credentialSchemaId);
            var schemaCreatorWalletData = await GetDefaultWalletDataAsync(schema.UserId);

            var walletData = await GetDefaultWalletDataAsync(userId);
            var wallet = await Wallet.OpenWalletAsync(walletData.Name, null, null);

            try
            {
                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {
                    var schemaData = JsonConvert.SerializeObject(new
                    {
                        name = schema.Name,
                        version = schema.Version
                    });

                    var getSchemaRequest = await Ledger.BuildGetSchemaRequestAsync(walletData.Did, schemaCreatorWalletData.Did, schemaData);
                    var getSchemaResponse = await Ledger.SubmitRequestAsync(pool, getSchemaRequest);
                    var schemaValue = JObject.Parse(getSchemaResponse).GetValue("result");
                    var claimDefJson = await AnonCreds.IssuerCreateAndStoreClaimDefAsync(wallet, walletData.Did, schemaValue.ToString(), "CL", false);
                    var claimDef = JObject.Parse(claimDefJson);
                    var claimDefRequest = await Ledger.BuildClaimDefTxnAsync(walletData.Did, claimDef.Value<int>("ref"), claimDef.Value<string>("signature_type"), claimDef.GetValue("data").ToString());
                    var result = await Ledger.SignAndSubmitRequestAsync(pool, wallet, walletData.Did, claimDefRequest);

                    var claimDefinition = new ClaimDefinition
                    {
                        Id = Guid.NewGuid(),
                        CredentialSchemaId = credentialSchemaId,
                        UserId = userId
                    };

                    _dbContext.ClaimDefinitions.Add(claimDefinition);
                    await _dbContext.SaveChangesAsync();
                    return claimDefinition;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await wallet.CloseAsync();
            }
        }

        public async Task<ClaimDefinition> GetClaimDefinitionAsync(Guid claimDefinitionId)
        {
            return await _dbContext.ClaimDefinitions
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == claimDefinitionId);
        }

        public async Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null)
        {
            var query = _dbContext.ClaimDefinitions
                .Include(p => p.User)
                .Include(p => p.CredentialSchema)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.UserId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task ApplyClaimDefinitionAsync(string userId, Guid claimDefinitionId)
        {
            var claimDefinition = await _dbContext.ClaimDefinitions.Include(p => p.CredentialSchema).FirstOrDefaultAsync(p => p.Id == claimDefinitionId);

            var issuerWalletData = await GetDefaultWalletDataAsync(claimDefinition.UserId);
            var issuerWallet = await Wallet.OpenWalletAsync(issuerWalletData.Name, null, null);

            var userWalletData = await GetDefaultWalletDataAsync(userId);
            var userWallet = await Wallet.OpenWalletAsync(userWalletData.Name, null, null);

            var schemaCreatorWalletData = await GetDefaultWalletDataAsync(claimDefinition.CredentialSchema.UserId);


            try
            {
                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {
                    var userIssuerDidResult = await Did.CreateAndStoreMyDidAsync(userWallet, "{}");
                    var schema = await GetSchemaAsync(pool, issuerWalletData.Did, schemaCreatorWalletData.Did, claimDefinition.CredentialSchema.Name, claimDefinition.CredentialSchema.Version);

                    var transcriptClaimOfferJson = await AnonCreds.IssuerCreateClaimOfferAsync(issuerWallet, schema.GetValue("result").ToString(), issuerWalletData.Did, userIssuerDidResult.Did);

                    await AnonCreds.ProverStoreClaimOfferAsync(userWallet, transcriptClaimOfferJson);

                    var userMasterSecretName = RandomUtils.RandomString(8);
                    await AnonCreds.ProverCreateMasterSecretAsync(userWallet, userMasterSecretName);

                    var getClaimDefRequest = await Ledger.BuildGetClaimDefTxnAsync(userIssuerDidResult.Did, schema.GetValue("result").Value<int>("seqNo"), "CL", issuerWalletData.Did);
                    var getClaimDefResponse = await Ledger.SubmitRequestAsync(pool, getClaimDefRequest);
                    var transcriptClaimDef = JObject.Parse(getClaimDefResponse).GetValue("result");

                    var transcriptClaimRequestJson = await AnonCreds.ProverCreateAndStoreClaimReqAsync(userWallet, userIssuerDidResult.Did, transcriptClaimOfferJson, transcriptClaimDef.ToString(), userMasterSecretName);

                    var userIndyClaim = new UserIndyClaim
                    {
                        ClaimDefinitionId = claimDefinitionId,
                        ClaimRequest = transcriptClaimRequestJson,
                        Id = Guid.NewGuid(),
                        LastUpdated = DateTime.UtcNow,
                        Status = UserIndyClaimStatus.Requested,
                        TimeCreated = DateTime.UtcNow,
                        UserId = userId
                    };

                    _dbContext.UserIndyClaims.Add(userIndyClaim);
                    await _dbContext.SaveChangesAsync();
                }
            }
            finally
            {
                await userWallet.CloseAsync();
                await issuerWallet.CloseAsync();

            }

        }

        public async Task AcceptClaimRequestAsync(string userId, Guid requestId, List<KeyValuePair<string, string>> attributeValues)
        {
            var walletData = await GetDefaultWalletDataAsync(userId);
            var wallet = await Wallet.OpenWalletAsync(walletData.Name, null, null);
            try
            {
                var userIndyClaim = await _dbContext.UserIndyClaims
                    .FirstOrDefaultAsync(p => p.Id == requestId);

                var attributes = new JObject();
                foreach (var attr in attributeValues)
                {
                    attributes.Add(attr.Key, attr.Value);
                }

                var claimJson = await AnonCreds.IssuerCreateClaimAsync(wallet, userIndyClaim.ClaimRequest, attributes.ToString(), -1);

                userIndyClaim.ClaimResponse = claimJson.ClaimJson;
                _dbContext.UpdateEntity(userIndyClaim);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await wallet.CloseAsync();
            }

        }

        private async Task<string> EncodeValue(string original, string key)
        {
            var encoded = await Crypto.AnonCryptAsync(key, Encoding.UTF8.GetBytes(original));
            return Encoding.UTF8.GetString(encoded);
        }

    }
}
