using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WaultBlock.Web.Controllers
{

    [Authorize]
    [Route("[controller]/[action]")]
    public class IdentitiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
