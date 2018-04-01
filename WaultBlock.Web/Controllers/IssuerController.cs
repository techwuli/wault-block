using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaultBlock.Identities;
using WaultBlock.Models;
using WaultBlock.Web.Models.IssuerViewModels;

namespace WaultBlock.Web.Controllers
{
    [Authorize]
    [Route("issuer")]
    public class IssuerController : Controller
    {
        private IWaultIdentityService _identityService;
        private UserManager<ApplicationUser> _userManager;

        public IssuerController(IWaultIdentityService identityService, UserManager<ApplicationUser> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        [HttpGet("ClaimDefinitions/Create")]
        public IActionResult CreateClaimDefinition()
        {
            return View();
        }

        [HttpPost("ClaimDefinitions/Create")]
        public async Task<IActionResult> CreateClaimDefinition(ClaimDefinitionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _identityService.CreateClaimDefinitionAsync(_userManager.GetUserId(User), model.Name, model.Fields);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            return View(model);
        }

        [HttpGet("ClaimDefinitions/{id}/Publish")]
        public async Task<IActionResult> PublishClaimDefinition(Guid id)
        {
            var claimDefinition = await _identityService.GetClaimDefinitionAsync(id);
            if (claimDefinition == null)
            {
                return NotFound();
            }
            var wallets = await _identityService.GetWalletDatasAsync(_userManager.GetUserId(User));
            ViewBag.Wallets = wallets;
            ViewBag.ClaimDefinition = ClaimDefinitionViewModel.Create(claimDefinition);
            return View();
        }

        [HttpPost("ClaimDefinitions/{id}/Publish")]
        public async Task<IActionResult> PublishClaimDefinition(Guid id, PublishClaimDefinitionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _identityService.PublishClaimDefinitionAsync(id, model.WalletName, _userManager.GetUserId(User));
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            var claimDefinition = await _identityService.GetClaimDefinitionAsync(id);
            if (claimDefinition == null)
            {
                return NotFound();
            }
            var wallets = await _identityService.GetWalletDatasAsync(_userManager.GetUserId(User));
            ViewBag.Wallets = wallets;
            ViewBag.ClaimDefinition = ClaimDefinitionViewModel.Create(claimDefinition);
            return View(model);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var defs = await _identityService.GetClaimDefinitionsAsync(_userManager.GetUserId(User));
            var result = defs.Select(ClaimDefinitionViewModel.Create).ToList();
            return View(result);
        }
    }
}
