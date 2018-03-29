using System.Collections.Generic;
using System.Threading.Tasks;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public interface IWaultIdentityService
    {
        Task<WaultWallet> CreateWalletAsync(string name, string userId);

        Task<IEnumerable<WaultWallet>> GetWalletsAsync(string userId);
    }
}
