using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UserIndyClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWaultIdentityService _identityService;

        public UserIndyClaimsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWaultIdentityService identityService)
        {
            _context = context;
            _userManager = userManager;
            _identityService = identityService;
        }

        // GET: UserIndyClaims/Create
        public IActionResult Create()
        {
            ViewData["ClaimDefinitionId"] = new SelectList(_context.ClaimDefinitions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        public async Task<IActionResult> Requests()
        {
            var userId = _userManager.GetUserId(User);
            var requests = await _context.UserIndyClaims
                .Include(p => p.User)
                .Include(p => p.ClaimDefinition)
                .Include(p => p.ClaimDefinition.CredentialSchema)
                .Where(p => p.ClaimDefinition.UserId == userId).ToListAsync();
            return View(requests);
        }

        [HttpGet("UserIndyClaims/{id}/Accept")]
        public async Task<IActionResult> AcceptRequest(Guid id)
        {
            var userIndyClaim = await _context.UserIndyClaims
                .Include(p => p.User)
                .Include(p => p.ClaimDefinition)
                .Include(p => p.ClaimDefinition.CredentialSchema)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (userIndyClaim == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (userIndyClaim.ClaimDefinition.UserId != userId)
            {
                return NotFound();
            }

            ViewData["Claim"] = userIndyClaim;

            return View();
        }

        [HttpPost("UserIndyClaims/{id}/Accept")]
        public async Task<IActionResult> AcceptRequest(Guid id, IFormCollection formData)
        {
            var userIndyClaim = await _context.UserIndyClaims
                .Include(p => p.User)
                .Include(p => p.ClaimDefinition)
                .Include(p => p.ClaimDefinition.CredentialSchema)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (userIndyClaim == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (userIndyClaim.ClaimDefinition.UserId != userId)
            {
                return NotFound();
            }

            try
            {
                var values = new Dictionary<string, string>();

                foreach (var key in formData.Keys)
                {
                    if (key.Equals("__RequestVerificationToken"))
                    {
                        continue;
                    }

                    values.Add(key, formData[key]);
                }

                await _identityService.AcceptClaimRequestAsync(userId, id, values);
                return RedirectToAction("Requests");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            ViewData["Claim"] = userIndyClaim;

            return View(formData);
        }

        // POST: UserIndyClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Issued,ClaimDefinitionId,UserId")] UserIndyClaim userIndyClaim)
        {
            if (ModelState.IsValid)
            {
                userIndyClaim.Id = Guid.NewGuid();
                _context.Add(userIndyClaim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClaimDefinitionId"] = new SelectList(_context.ClaimDefinitions, "Id", "Id", userIndyClaim.ClaimDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userIndyClaim.UserId);
            return View(userIndyClaim);
        }

        // GET: UserIndyClaims/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIndyClaim = await _context.UserIndyClaims
                .Include(u => u.ClaimDefinition)
                .Include(u => u.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userIndyClaim == null)
            {
                return NotFound();
            }

            return View(userIndyClaim);
        }

        // POST: UserIndyClaims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userIndyClaim = await _context.UserIndyClaims.SingleOrDefaultAsync(m => m.Id == id);
            _context.UserIndyClaims.Remove(userIndyClaim);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: UserIndyClaims/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIndyClaim = await _context.UserIndyClaims
                .Include(u => u.ClaimDefinition)
                .Include(u => u.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (userIndyClaim == null)
            {
                return NotFound();
            }

            return View(userIndyClaim);
        }

        // GET: UserIndyClaims/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIndyClaim = await _context.UserIndyClaims.SingleOrDefaultAsync(m => m.Id == id);
            if (userIndyClaim == null)
            {
                return NotFound();
            }
            ViewData["ClaimDefinitionId"] = new SelectList(_context.ClaimDefinitions, "Id", "Id", userIndyClaim.ClaimDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userIndyClaim.UserId);
            return View(userIndyClaim);
        }

        // POST: UserIndyClaims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Issued,ClaimDefinitionId,UserId")] UserIndyClaim userIndyClaim)
        {
            if (id != userIndyClaim.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userIndyClaim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserIndyClaimExists(userIndyClaim.Id))
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
            ViewData["ClaimDefinitionId"] = new SelectList(_context.ClaimDefinitions, "Id", "Id", userIndyClaim.ClaimDefinitionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userIndyClaim.UserId);
            return View(userIndyClaim);
        }

        // GET: UserIndyClaims
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var applicationDbContext = _context.UserIndyClaims
                .Include(u => u.ClaimDefinition)
                .Include(u => u.ClaimDefinition.User)
                .Include(u => u.ClaimDefinition.CredentialSchema)
                .Include(u => u.User)
                .Where(p => p.UserId == userId);

            return View(await applicationDbContext.ToListAsync());
        }

        private bool UserIndyClaimExists(Guid id)
        {
            return _context.UserIndyClaims.Any(e => e.Id == id);
        }
    }
}
