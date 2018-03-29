using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WaultBlock.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<WaultWallet> WaultWallets { get; set; }
    }
}
