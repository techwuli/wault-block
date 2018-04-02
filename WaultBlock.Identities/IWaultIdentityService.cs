using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public interface IWaultIdentityService
    {
        #region wallets

        Task CreateWalletForBusinessAsync(ApplicationUser user);

        Task CreateWalletForUserAsync(ApplicationUser user, string seed = null, bool isDefault = false);

        Task<IEnumerable<WalletData>> GetWalletDatasAsync(string userId);

        #endregion wallets

        #region claims

        Task CreateCredentialSchemaAsync(string userId, string name, string version, string[] attributes);

        Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, Guid credentialSchemaId);

        Task<ClaimDefinition> GetClaimDefinitionAsync(Guid claimDefinitionId);

        Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null);

        Task ApplyClaimDefinitionAsync(string userId, Guid claimDefinitionId);

        #endregion claims
    }
}
