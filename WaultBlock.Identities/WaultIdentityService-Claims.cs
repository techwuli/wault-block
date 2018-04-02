using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaultBlock.Identities.Utils;
using WaultBlock.Models;

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
                    var schemaClaimDefJson = await AnonCreds.IssuerCreateAndStoreClaimDefAsync(wallet, walletData.Did, schemaValue.ToString(), "CL", false);
                    var claimDef = JObject.Parse(schemaClaimDefJson);
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

            var trustAnchorWalletData = await GetDefaultWalletDataAsync(claimDefinition.UserId);
            var userWalletData = await GetDefaultWalletDataAsync(userId);

            var userWallet = await Wallet.OpenWalletAsync(userWalletData.Name, null, null);
            var trustAnchorWallet = await Wallet.OpenWalletAsync(trustAnchorWalletData.Name, null, null);

            try
            {
                using (var pool = await PoolUtils.CreateAndOpenPoolLedgerAsync())
                {

                    var userFaberDidResult = await Did.CreateAndStoreMyDidAsync(userWallet, "{}");

                    var transcriptSchema = JsonConvert.SerializeObject(new { name = claimDefinition.CredentialSchema.Name, version = claimDefinition.CredentialSchema.Version });
                    var claimOfferJson = await AnonCreds.IssuerCreateClaimOfferAsync(trustAnchorWallet, transcriptSchema, trustAnchorWalletData.Did, userFaberDidResult.Did);

                    await AnonCreds.ProverStoreClaimOfferAsync(userWallet, claimOfferJson);
                    var getSchemaRequest = await Ledger.BuildGetSchemaRequestAsync(userFaberDidResult.Did, trustAnchorWalletData.Did, transcriptSchema);
                    var getSchemaResponse = await Ledger.SubmitRequestAsync(pool, getSchemaRequest);
                    var transscriptSchemaObj = JObject.Parse(getSchemaResponse).GetValue("result");

                    var userMasterSecretName = "user_master_secret";
                    await AnonCreds.ProverCreateMasterSecretAsync(userWallet, userMasterSecretName);

                    var getClaimDefRequest = await Ledger.BuildGetClaimDefTxnAsync(userFaberDidResult.Did, transscriptSchemaObj.Value<int>("seqNo"), "CL", trustAnchorWalletData.Did);
                    var getClaimDefResponse = await Ledger.SubmitRequestAsync(pool, getClaimDefRequest);

                    var transcriptClaimDef = JObject.Parse(getClaimDefResponse).GetValue("result");


                    var transcriptClaimRequestJson = await AnonCreds.ProverCreateAndStoreClaimReqAsync(userWallet, userFaberDidResult.Did, claimOfferJson, transcriptClaimDef.ToString(), userMasterSecretName);



                }
            }
            finally
            {
                await userWallet.CloseAsync();
                await trustAnchorWallet.CloseAsync();

            }

        }
    }
}
