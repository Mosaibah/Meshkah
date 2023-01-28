using Meshkah.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Meshkah.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly MeshkahContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int userId;
        private string userName;


        public StudentController(MeshkahContext context, IHttpContextAccessor httpContextAccessor)
        {
            //int userId = Convert.ToInt32(User.FindFirst("UserId").Value);
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            var claim = _httpContextAccessor.HttpContext;
            userId = Convert.ToInt32(claim.User.FindFirstValue("UserId"));
            userName = claim.User.FindFirstValue("Name");
        }

        public async Task<IActionResult> Index()
        {
            // total points
            // total money
            // points history
            // money history
            var pointsTotal = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId).SumAsync(c => c.Point.Amount);
            var moneyTotal = await _context.MoneyMovements.Where(c => c.UserId == userId).SumAsync(c => c.Amount);

            var pointsLog = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId)
                            .Select(c => new { Name = c.Point.Name, Amount = c.Point.Name, CreatedAt = c.CreatedAt }).ToListAsync();

            ViewData["pointsTotal"] = pointsTotal;
            ViewData["moneyTotal"] = moneyTotal;

            //var moneylog = await _context.moneymovements.include(c => c.point).where(c => c.userid == userid)
            //                .select(c => new { name = c.point.name, amount = c.point.name, createdat = c.createdat }).tolistasync();



            return View();
        }
    }
}
