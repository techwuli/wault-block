using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public interface IWaultIdentityService
    {
        #region wallets

        Task CreateWalletAsync(ApplicationUser user, string agentId = null);

        Task<IEnumerable<WalletData>> GetWalletDatasAsync(string userId);

        #endregion wallets

        #region claims

        Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, string name, string value);

        Task<ClaimDefinition> GetClaimDefinitionAsync(Guid claimDefinitionId);

        Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null, bool? published = null);

        Task PublishClaimDefinitionAsync(Guid claimDefinitionId, string walletName, string userId);

        #endregion claims
    }
}
