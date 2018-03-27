using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WaultBlock.Web.Services;

namespace WaultBlock.Web.Controllers
{

    [Authorize]
    [Route("Claims")]
    public class ClaimsController : Controller
    {
        private IIndyService _indyService;

        public ClaimsController(IIndyService indyService)
        {
            _indyService = indyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claims = await _indyService.GetClaimsAsync(User);
            return View(claims);
        }
    }
}
