using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WaultBlock.Data;
using WaultBlock.Identities;
using WaultBlock.Models;

namespace WaultBlock.Sample.Controllers
{
    [Authorize]
    public class ClaimDefinitionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWaultIdentityService _identityService;

        public ClaimDefinitionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWaultIdentityService identityService)
        {
            _context = context;
            _userManager = userManager;
            _identityService = identityService;
        }

        // GET: ClaimDefinitions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ClaimDefinitions.Include(c => c.CredentialSchema).Include(c => c.User).Where(p => p.UserId == _userManager.GetUserId(User));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ClaimDefinitions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimDefinition = await _context.ClaimDefinitions
                .Include(c => c.CredentialSchema)
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (claimDefinition == null)
            {
                return NotFound();
            }

            return View(claimDefinition);
        }

        // GET: ClaimDefinitions/Create
        public IActionResult Create()
        {
            ViewData["CredentialSchemaId"] = new SelectList(_context.CredentialSchemas, "Id", "Attributes");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ClaimDefinitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CredentialSchemaId")] ClaimDefinition claimDefinition)
        {
            if (ModelState.IsValid)
            {

                await _identityService.CreateClaimDefinitionAsync(_userManager.GetUserId(User), claimDefinition.CredentialSchemaId);

                return RedirectToAction(nameof(Index));
            }
            ViewData["CredentialSchemaId"] = new SelectList(_context.CredentialSchemas, "Id", "Attributes", claimDefinition.CredentialSchemaId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", claimDefinition.UserId);
            return View(claimDefinition);
        }

        // GET: ClaimDefinitions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimDefinition = await _context.ClaimDefinitions.SingleOrDefaultAsync(m => m.Id == id);
            if (claimDefinition == null)
            {
                return NotFound();
            }
            ViewData["CredentialSchemaId"] = new SelectList(_context.CredentialSchemas, "Id", "Attributes", claimDefinition.CredentialSchemaId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", claimDefinition.UserId);
            return View(claimDefinition);
        }

        // POST: ClaimDefinitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CredentialSchemaId,UserId")] ClaimDefinition claimDefinition)
        {
            if (id != claimDefinition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claimDefinition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimDefinitionExists(claimDefinition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CredentialSchemaId"] = new SelectList(_context.CredentialSchemas, "Id", "Attributes", claimDefinition.CredentialSchemaId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", claimDefinition.UserId);
            return View(claimDefinition);
        }

        // GET: ClaimDefinitions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimDefinition = await _context.ClaimDefinitions
                .Include(c => c.CredentialSchema)
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (claimDefinition == null)
            {
                return NotFound();
            }

            return View(claimDefinition);
        }

        // POST: ClaimDefinitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var claimDefinition = await _context.ClaimDefinitions.SingleOrDefaultAsync(m => m.Id == id);
            _context.ClaimDefinitions.Remove(claimDefinition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("ClaimDefinitions/{id}/Apply")]
        public async Task<IActionResult> Apply(Guid id)
        {
            await _identityService.ApplyClaimDefinitionAsync(_userManager.GetUserId(User), id);
            return RedirectToAction("Index", "UserIndyClaims");
        }

        private bool ClaimDefinitionExists(Guid id)
        {
            return _context.ClaimDefinitions.Any(e => e.Id == id);
        }
    }
}
