using System;
using System.Collections.Generic;
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

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var defs = await _identityService.GetClaimDefinitionsAsync(_userManager.GetUserId(User));

            var result = new List<ClaimDefinitionViewModel>();

            foreach (var def in defs)
            {
                var model = new ClaimDefinitionViewModel
                {
                    Fields = def.Fields,
                    Name = def.Name,
                    UserName = def.User.UserName,
                    Published = def.Published
                };
                result.Add(model);
            }
            return View(result);
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
    }
}
