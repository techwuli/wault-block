using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WaultBlock.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<WalletData> WalletDatas { get; set; }

        public virtual ICollection<ClaimDefinition> ClaimDefinitions { get; set; }

        public virtual ICollection<CredentialSchema> CredentialSchemas { get; set; }
    }
}
