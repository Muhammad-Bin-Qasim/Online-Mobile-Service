using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Data;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(ApplicationDbContext context,
             UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Redirect("/Identity/Account/Login");

            ViewBag.DND = await _context.ServiceActivations
                .AnyAsync(s => s.UserId == user.Id &&
                               s.ServiceName == "DND" &&
                               s.Status == "Active");

            ViewBag.CallerTune = await _context.ServiceActivations
                .AnyAsync(s => s.UserId == user.Id &&
                               s.ServiceName == "CallerTune" &&
                               s.Status == "Active");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateServices(bool dnd, bool callerTune)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Redirect("/Identity/Account/Login");

            // 🔥 DO NOT DISTURB
            var dndService = await _context.ServiceActivations
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.ServiceName == "DND");

            if (dnd)
            {
                if (dndService == null)
                {
                    _context.ServiceActivations.Add(new ServiceActivation
                    {
                        UserId = user.Id,
                        ServiceName = "DND",
                        Status = "Active"
                    });
                }
                else
                {
                    dndService.Status = "Active";
                }
            }
            else if (dndService != null)
            {
                dndService.Status = "Inactive";
            }

            // 🔥 CALLER TUNE
            var callerService = await _context.ServiceActivations
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.ServiceName == "CallerTune");

            if (callerTune)
            {
                if (callerService == null)
                {
                    _context.ServiceActivations.Add(new ServiceActivation
                    {
                        UserId = user.Id,
                        ServiceName = "CallerTune",
                        Status = "Active"
                    });
                }
                else
                {
                    callerService.Status = "Active";
                }
            }
            else if (callerService != null)
            {
                callerService.Status = "Inactive";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Services updated successfully!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Redirect("/Identity/Account/Login");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Redirect("/Identity/Account/Login");

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profile updated successfully!";

            return RedirectToAction("Index");
        }

    }

}
