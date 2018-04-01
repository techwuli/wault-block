using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using WaultBlock.Identities.DataObjects;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public partial class WaultIdentityService
    {
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

        public async Task<ClaimDefinition> GetClaimDefinitionAsync(Guid claimDefinitionId)
        {
            return await _dbContext.ClaimDefinitions
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == claimDefinitionId);
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

        public async Task PublishClaimDefinitionAsync(Guid claimDefinitionId, string walletName, string userId)
        {
            var claimDefinition = await _dbContext.ClaimDefinitions.FirstOrDefaultAsync(p => p.Id == claimDefinitionId && p.UserId == userId);
            if (claimDefinition == null)
            {
                throw new Exception("Claim definition not found.");
            }

            // var waultWallet = await _dbContext.WaultWallets.FirstOrDefaultAsync(p => p.Name == walletName && p.UserId == userId);
            // await Wallet.RegisterWalletTypeAsync(InDbWalletType.WAULT_TYPE, new InDbWalletType(_dbContext));
            Wallet indyWallet = null;
            try
            {
                indyWallet = await OpenWalletAsync(walletName, null, "{\"userId\":\"" + userId + "\"}");
                if (indyWallet == null)
                {
                    throw new Exception("Wallet can not be opened.");
                }

                var waultWallet = await GetWalletDataAsync(walletName, userId);

                var claimDefinitionObj = new ClaimDefinitionObject
                {
                    SequenceNumber = 1,
                    Data = new ClaimDefinitionObjectData
                    {
                        AttributeNames = claimDefinition.FieldArray,
                        Name = claimDefinition.Name,
                        Version = "1.0"
                    }
                };
                // AnonCreds.IssuerCreateClaimOfferAsync(indyWallet)
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (indyWallet != null)
                {
                    await indyWallet.CloseAsync();
                }
            }

            //  AnonCreds.IssuerCreateAndStoreClaimDefAsync(indyWallet, Did.)
        }
    }
}
