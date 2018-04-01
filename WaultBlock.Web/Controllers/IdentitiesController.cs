using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaultBlock.Identities;
using WaultBlock.Models;

namespace WaultBlock.Web.Controllers
{
    [Authorize]
    [Route("Identities")]
    public class IdentitiesController : Controller
    {
        private IWaultIdentityService _identityService;
        private UserManager<ApplicationUser> _userManager;

        public IdentitiesController(UserManager<ApplicationUser> userManager, IWaultIdentityService identityService)
        {
            _userManager = userManager;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var wallets = await _identityService.GetWalletDatasAsync(_userManager.GetUserId(User));

            return View(wallets);
        }
    }
}
