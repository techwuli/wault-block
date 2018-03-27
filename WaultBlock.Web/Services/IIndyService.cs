using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WaultBlock.Web.Models.ClaimsViewModels;

namespace WaultBlock.Web.Services
{
    public interface IIndyService
    {
        Task<List<IndyClaimViewModel>> GetClaimsAsync(ClaimsPrincipal user);
    }
}
