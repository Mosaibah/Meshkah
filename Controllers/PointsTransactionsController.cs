using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Meshkah.Models;
using Meshkah.ViewModels;

namespace Meshkah.Controllers
{
    public class PointsTransactionsController : Controller
    {
        private readonly MeshkahContext _context;

        public PointsTransactionsController(MeshkahContext context)
        {
            _context = context;
        }

        // GET: PointsTransactions
        public async Task<IActionResult> Index()
        {
            var meshkahContext = _context.PointsTransactions.Include(p => p.Point).Include(p => p.User);
            return View(await meshkahContext.ToListAsync());
        }

        // GET: PointsTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PointsTransactions == null)
            {
                return NotFound();
            }

            var pointsTransaction = await _context.PointsTransactions
                .Include(p => p.Point)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointsTransaction == null)
            {
                return NotFound();
            }

            return View(pointsTransaction);
        }

        // GET: PointsTransactions/Create
        public IActionResult Create()
        {
            ViewData["PointId"] = new SelectList(_context.Points, "Id", "Name");
            ViewData["UserId"] = new MultiSelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: PointsTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PointId,Selected_Users,CreatedAt")] PointsTransactionVM model)
        {
            if (ModelState.IsValid)
            {
                foreach(int user in model.Selected_Users)
                {
                    _context.Add(new PointsTransaction() { PointId= model.PointId, UserId = user});
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PointId"] = new SelectList(_context.Points, "Id", "Id", model.PointId);
            ViewData["UserId"] = new MultiSelectList(_context.Users, "Id", "Id", model.Selected_Users);
            return View(model);
        }

        // GET: PointsTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PointsTransactions == null)
            {
                return NotFound();
            }

            var pointsTransaction = await _context.PointsTransactions.FindAsync(id);
            if (pointsTransaction == null)
            {
                return NotFound();
            }
            ViewData["PointId"] = new SelectList(_context.Points, "Id", "Name", pointsTransaction.PointId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", pointsTransaction.UserId);
            return View(pointsTransaction);
        }

        // POST: PointsTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PointId,UserId,CreatedAt")] PointsTransaction pointsTransaction)
        {
            if (id != pointsTransaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pointsTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointsTransactionExists(pointsTransaction.Id))
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
            ViewData["PointId"] = new SelectList(_context.Points, "Id", "Name", pointsTransaction.PointId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", pointsTransaction.UserId);
            return View(pointsTransaction);
        }

        // GET: PointsTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PointsTransactions == null)
            {
                return NotFound();
            }

            var pointsTransaction = await _context.PointsTransactions
                .Include(p => p.Point)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointsTransaction == null)
            {
                return NotFound();
            }

            return View(pointsTransaction);
        }

        // POST: PointsTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PointsTransactions == null)
            {
                return Problem("Entity set 'MeshkahContext.PointsTransactions'  is null.");
            }
            var pointsTransaction = await _context.PointsTransactions.FindAsync(id);
            if (pointsTransaction != null)
            {
                _context.PointsTransactions.Remove(pointsTransaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointsTransactionExists(int id)
        {
          return _context.PointsTransactions.Any(e => e.Id == id);
        }
    }
}
