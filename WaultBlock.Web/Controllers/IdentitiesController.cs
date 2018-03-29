using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaultBlock.Identities;
using WaultBlock.Models;
using WaultBlock.Web.Models.IdentitiesViewModels;

namespace WaultBlock.Web.Controllers
{
    [Authorize]
    [Route("Claims")]
    public class IdentitiesController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IWaultIdentityService _identityService;

        public IdentitiesController(UserManager<ApplicationUser> userManager, IWaultIdentityService identityService)
        {
            _userManager = userManager;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var wallets = await _identityService.GetWalletsAsync(_userManager.GetUserId(User));

            return View(wallets);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateWalletViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _identityService.CreateWalletAsync(model.Name, _userManager.GetUserId(User));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
