﻿using Meshkah.Models;
using Meshkah.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            ViewData["pointsTotal"] = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId).SumAsync(c => c.Point.Amount);

            ViewData["individualPoints"] = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId && c.Point.TypeId == 1).SumAsync(c => c.Point.Amount);

            ViewData["groupPoints"] = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId && c.Point.TypeId == 2).SumAsync(c => c.Point.Amount);
            
            ViewData["moneyTotal"] = await _context.MoneyMovements.Where(c => c.UserId == userId).SumAsync(c => c.Amount);


           
            



            return View();
        }

        public async Task<IActionResult> TransferMoney()
        {
            // get amount of user available
            // get all students (id, name)

            var availableAmount = await _context.MoneyMovements.Where(c => c.UserId == userId).SumAsync(c => c.Amount);

            List<int?> usersId = await _context.UserRoleMappings.Where(c => c.RoleId == 3 && c.UserId != userId).Select(c => c.UserId).ToListAsync();
            var students = await _context.Users.Where(c => usersId.Contains(c.Id)).Select(c => new { c.Id, c.Name }).ToListAsync();

            ViewData["availableAmount"] = availableAmount;
            ViewData["students"] = new SelectList(students, "Id", "Name");

            return View();
        }

        #region POST: TransferMoney
        [HttpPost]
        public async Task<IActionResult> TransferMoney([Bind("Amount, ToUserId")] TransferMoneyVM model)
        {
            model.FromUserId = userId;
            if (ModelState.IsValid)
            {
                var moneyTransaction = new MoneyTransaction()
                {
                    Amount = model.Amount,
                    FromUserId = model.FromUserId,
                    ToUserId = model.ToUserId,

                };
                await _context.MoneyTransactions.AddAsync(moneyTransaction);
                await _context.SaveChangesAsync();

                _context.Add(new MoneyMovement()
                {
                    UserId = model.FromUserId,
                    Amount = -model.Amount,
                    MoneyTransactionId = moneyTransaction.Id

                });
                _context.Add(new MoneyMovement()
                {
                    UserId = model.ToUserId,
                    Amount = model.Amount,
                    MoneyTransactionId = moneyTransaction.Id
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        #endregion

        public async Task<IActionResult> PointsHistory()
        {
            ViewData["pointsLog"] = await _context.PointsTransactions.Include(c => c.Point).Where(c => c.UserId == userId)
                            .Select(c => new { Name = c.Point.Name, Amount = c.Point.Amount, CreatedAt = c.CreatedAt }).ToListAsync();

            return View();
        }
        public async Task<IActionResult> MoneyHistory()
        {
            //ViewData["moneyLog"] = await _context.MoneyMovements.Include(c => c.Point).Include(c => c.MoneyTransaction)
            //               .Where(c => c.UserId == userId)
            //               .Select(c => new 
            //               { 
            //                   Amount = c.Amount, 
            //                   PointName = c.Point.Name,
            //                   From = c.MoneyTransaction.FromUserId.ToString(),
            //                   To = c.MoneyTransaction.ToUserId.ToString(),
            //                   CreatedAt = c.CreatedAt 
            //               })
            //               .ToListAsync();
            ViewData["transferHistory"] = await _context.MoneyTransactions
                                        .Include(c => c.FromUser)
                                        .Include(c => c.ToUser)
                           .Where(c => c.FromUserId == userId || c.ToUserId == userId)
                           .Select(c => new
                           {
                               Amount = c.Amount,
                               FromUser = c.FromUser.Name,
                               ToUser = c.ToUser.Name,
                               CreatedAt = c.CreatedAt,
                               IsFromMe = c.FromUserId == userId ? true : false
                           })
                           .ToListAsync();

            return View();
        }
        
    }
}
