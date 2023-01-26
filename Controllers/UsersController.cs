using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Meshkah.Models;
using Meshkah.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Meshkah.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly MeshkahContext _context;

        public UsersController(MeshkahContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            List<User> users = await _context.Users.Include(c => c.UserGroupMappings).ThenInclude(c => c.Group).Include(c => c.UserRoleMappings).ThenInclude(c => c.Role).ToListAsync();
             return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Groups"] = new MultiSelectList(_context.Groups, "Id", "Name");
            ViewData["Roles"] = new MultiSelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name, Selected_Groups, Selected_Roles")] CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                User user = new User() { Name = model.Name };
                _context.Add(user);
                await _context.SaveChangesAsync();

                foreach(int groupId in model.Selected_Groups)
                {
                    _context.Add(new UserGroupMapping() { UserId = user.Id, GroupId = groupId });
                }
                foreach(int roleId in model.Selected_Roles)
                {
                    _context.Add(new UserRoleMapping() { UserId = user.Id, RoleId = roleId });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Include(c => c.UserGroupMappings).ThenInclude(c => c.Group).Include(c => c.UserRoleMappings).ThenInclude(c => c.Role)
                .Where( c => c.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            var selectedGroups = user.UserGroupMappings.Select(c => c.GroupId).ToList();
            List<Group> groups = await _context.Groups.ToListAsync();
            
            var selectedRoles = user.UserRoleMappings.Select(c => c.RoleId).ToList();
            List<Role> roles = await _context.Roles.ToListAsync();

            ViewData["Groups"] = new MultiSelectList(groups, "Id", "Name", selectedGroups);
            ViewData["Roles"] = new MultiSelectList(roles, "Id", "Name", selectedRoles);

            return View(new CreateUserVM() { Id = id.Value, Name = user.Name});
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name, Selected_Groups, Selected_Roles")] CreateUserVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(new User() { Id = id, Name = model.Name});

                    _context.UserGroupMappings.RemoveRange(_context.UserGroupMappings.Where(c => c.UserId == id));
                    _context.UserRoleMappings.RemoveRange(_context.UserRoleMappings.Where(c => c.UserId == id));

                    foreach (int groupId in model.Selected_Groups)
                    {
                        _context.Add(new UserGroupMapping() { UserId = id, GroupId = groupId });
                    }
                    foreach (int roleId in model.Selected_Roles)
                    {
                        _context.Add(new UserRoleMapping() { UserId = id, RoleId = roleId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.Id))
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
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'MeshkahContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return _context.Users.Any(e => e.Id == id);
        }
    }
}
