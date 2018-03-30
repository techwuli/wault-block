using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public interface IWaultIdentityService
    {
        Task<WaultWallet> CreateWalletAsync(string name, string userId);

        Task<IEnumerable<WaultWallet>> GetWalletsAsync(string userId);

        Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null, bool? published=null);

        Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, string name, string value);

        Task PublishClaimDefinitionAsync(Guid id);
    }
}
