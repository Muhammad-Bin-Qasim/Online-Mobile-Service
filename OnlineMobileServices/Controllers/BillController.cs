using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Data;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BillController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        // STEP 1 → Show Payment Page
        [HttpPost]
        public IActionResult Payment(string mobile, decimal amount)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                ModelState.AddModelError("", "Enter valid mobile number");
                return View("Index");
            }

            ViewBag.Mobile = mobile;
            ViewBag.Amount = amount;

            return View();
        }

        // STEP 2 → Confirm Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBill(string mobile, decimal amount)
        {
            var userId = User.Identity.IsAuthenticated
                ? _userManager.GetUserId(User)
                : null;

            var transaction = new Transaction
            {
                MobileNumber = mobile,
                Amount = amount,
                TransactionType = "Bill",
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
                    ActivityType = "Bill Payment",
                    Description = "Bill Payment of Rs " + amount,
                    Amount = amount,
                    Status = "Paid",
                    Date = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return View("Receipt", transaction);
        }
    }
}