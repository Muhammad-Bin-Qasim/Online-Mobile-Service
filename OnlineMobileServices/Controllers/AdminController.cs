using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Data;
using OnlineMobileServices.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMobileServices.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;

            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalTransactions = await _context.Transactions.CountAsync();
            ViewBag.TotalRevenue = await _context.Transactions.SumAsync(t => t.Amount);

            ViewBag.TodayTransactions = await _context.Transactions
                .Where(t => t.Date.Date == today)
                .CountAsync();

            var transactions = await _context.Transactions
                .Include(t => t.User)   // 🔥 ADD THIS LINE
                .OrderByDescending(t => t.Date)
                .Take(20)
                .ToListAsync();

            return View(transactions);
        }
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var transactionCount = await _context.Transactions
                    .CountAsync(t => t.UserId == user.Id);

                userList.Add(new
                {
                    user.Id,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "User",
                    TransactionCount = transactionCount,
                    IsBlocked = user.LockoutEnd != null &&
                                user.LockoutEnd > DateTimeOffset.UtcNow
                });
            }

            return View(userList);
        }


        public async Task<IActionResult> EditTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTransaction(Transaction model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == model.TransactionId);

            if (transaction == null)
                return NotFound();

            // Update ONLY allowed fields
            transaction.MobileNumber = model.MobileNumber;
            transaction.Amount = model.Amount;
            transaction.TransactionType = model.TransactionType;
            transaction.Status = model.Status;

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> UserHistory(string userId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.UserId = userId;

            return View(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Prevent admin from deleting themselves
            if (User.Identity.Name == user.Email)
                return RedirectToAction("Users");

            await _userManager.DeleteAsync(user);

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Users");
        }

    }
}
