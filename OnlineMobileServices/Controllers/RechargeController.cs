using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Data;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    
    public class RechargeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RechargeController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Plans(string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || mobile.Length != 10)
            {
                ModelState.AddModelError("", "Enter valid 10 digit mobile number");
                return View("Index");
            }

            ViewBag.Mobile = mobile;
            return View(_context.RechargePlans.ToList());
        }

        // STEP 1 → Show Payment Page
        public async Task<IActionResult> Pay(int id, string mobile)
        {
            var plan = await _context.RechargePlans.FindAsync(id);
            if (plan == null)
                return NotFound();

            ViewBag.Mobile = mobile;
            return View(plan);   // 🔥 Show Payment Page
        }

        // STEP 2 → Confirm Payment and Save Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRecharge(int planId, string mobile)
        {
            var plan = await _context.RechargePlans.FindAsync(planId);
            if (plan == null)
                return NotFound();

            var userId = User.Identity.IsAuthenticated
                ? _userManager.GetUserId(User)
                : null;

            var transaction = new Transaction
            {
                MobileNumber = mobile,
                Amount = plan.Amount,
                TransactionType = "Recharge",
                Status = "Paid",
                Date = DateTime.Now,
                UserId = userId
            };

            _context.Transactions.Add(transaction);

            if (userId != null)
            {
                _context.Activities.Add(new ActivityHistory
                {
                    UserId = userId,
                    ActivityType = "Recharge",
                    Description = "Recharge of Rs " + plan.Amount,
                    Amount = plan.Amount,
                    Status = "Paid",
                    Date = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return View("Receipt", transaction);
        }
    }
}