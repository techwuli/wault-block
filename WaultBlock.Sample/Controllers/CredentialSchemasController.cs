using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaultBlock.Data;
using WaultBlock.Identities;
using WaultBlock.Models;

namespace WaultBlock.Sample.Controllers
{
    [Authorize]
    public class CredentialSchemasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWaultIdentityService _identityService;

        public CredentialSchemasController(ApplicationDbContext context, IWaultIdentityService identityService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _identityService = identityService;
            _userManager = userManager;
        }

        // GET: CredentialSchemas
        public async Task<IActionResult> Index()
        {
            return View(await _context.CredentialSchemas.Where(p => p.UserId == _userManager.GetUserId(User)).ToListAsync());
        }

        // GET: CredentialSchemas/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSchema = await _context.CredentialSchemas
                .SingleOrDefaultAsync(m => m.Id == id);
            if (credentialSchema == null)
            {
                return NotFound();
            }

            return View(credentialSchema);
        }

        // GET: CredentialSchemas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CredentialSchemas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Version,Attributes")] CredentialSchema credentialSchema)
        {
            if (ModelState.IsValid)
            {
                await _identityService.CreateCredentialSchemaAsync(_userManager.GetUserId(User), credentialSchema.Name, credentialSchema.Version, credentialSchema.AttributeArray);

                return RedirectToAction(nameof(Index));
            }
            return View(credentialSchema);
        }

        // GET: CredentialSchemas/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSchema = await _context.CredentialSchemas.SingleOrDefaultAsync(m => m.Id == id);
            if (credentialSchema == null)
            {
                return NotFound();
            }
            return View(credentialSchema);
        }

        // POST: CredentialSchemas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Did,Name,Version,Attributes,UserId")] CredentialSchema credentialSchema)
        {
            if (id != credentialSchema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credentialSchema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialSchemaExists(credentialSchema.Id))
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
            return View(credentialSchema);
        }

        // GET: CredentialSchemas/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credentialSchema = await _context.CredentialSchemas
                .SingleOrDefaultAsync(m => m.Id == id);
            if (credentialSchema == null)
            {
                return NotFound();
            }

            return View(credentialSchema);
        }

        // POST: CredentialSchemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var credentialSchema = await _context.CredentialSchemas.SingleOrDefaultAsync(m => m.Id == id);
            _context.CredentialSchemas.Remove(credentialSchema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialSchemaExists(Guid id)
        {
            return _context.CredentialSchemas.Any(e => e.Id == id);
        }
    }
}
