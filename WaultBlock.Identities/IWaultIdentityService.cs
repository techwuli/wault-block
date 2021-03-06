﻿using System;
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

        Task AcceptClaimRequestAsync(string userId, Guid requestId, Dictionary<string, string> attributeValues);

        Task ApplyClaimDefinitionAsync(string userId, Guid claimDefinitionId);

        Task<ClaimDefinition> CreateClaimDefinitionAsync(string userId, Guid credentialSchemaId);

        Task CreateCredentialSchemaAsync(string userId, string name, string version, string[] attributes);

        Task<ClaimDefinition> GetClaimDefinitionAsync(Guid claimDefinitionId);

        Task<IEnumerable<ClaimDefinition>> GetClaimDefinitionsAsync(string userId = null);

        #endregion claims
    }
}
