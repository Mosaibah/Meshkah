﻿using System;
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
            //List<User> users = await _context.Users.Include(c => c.UserGroupMappings).ThenInclude(c => c.Group).Include(c => c.UserRoleMappings).ThenInclude(c => c.Role).ToListAsync();
             return View();
        }

        [HttpGet]
        [Route("users/list")]
        public async Task<IActionResult> List()
        {
            var users = await _context.Users.Include(c => c.UserRoleMappings).ThenInclude(c => c.Role)
                .Include(c => c.UserGroupMappings).ThenInclude(c => c.Group).ToListAsync();


            return Json(new { data = users.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                email = c.Email
            }).ToList()
            });
        }


        public async Task<IActionResult> ListUsers(SearchUsersVM model)
        {
            var pageSize = Convert.ToInt32(HttpContext.Request.Form["length"].FirstOrDefault() ?? "0");
            var skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var draw = Request.Form["draw"].FirstOrDefault();

            List<User> users = new();
            var recordsTotal = await _context.Users.CountAsync();
            var recordsFiltered = 0;

            var countQuotes = await _context.Users.CountAsync();
            var usersFiltered = _context.Users.Include(c => c.UserRoleMappings).ThenInclude(c => c.Role).Include(c => c.UserGroupMappings).ThenInclude(c => c.Group).AsQueryable();

            //if (model.Authors is not null)
            //{
            //    quotesFiltered = quotesFiltered.Where(x => model.Authors.Contains(x.AuthorId.Value));
            //}
            //if (model.Text is not null)
            //{
            //    quotesFiltered = quotesFiltered.Where(c => c.Text.Contains(model.Text));
            //}
            //if (model.StartDate is not null)
            //{
            //    quotesFiltered = quotesFiltered.Where(c => c.CreatedAt > model.StartDate);
            //}
            //if (model.EndDate is not null)
            //{
            //    quotesFiltered = quotesFiltered.Where(c => c.CreatedAt < model.EndDate.Value.AddDays(1));
            //}

            users = await usersFiltered.Skip(skip).Take(pageSize).ToListAsync();
            recordsFiltered = await usersFiltered.CountAsync();

            return Json(new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = users.Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    email = c.Email
                }).ToList()
            });
        }

        #region Details 
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
        #endregion

        #region GET: Create
        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Groups"] = new MultiSelectList(_context.Groups, "Id", "Name");
            ViewData["Roles"] = new MultiSelectList(_context.Roles, "Id", "Name");
            return View();
        }
        #endregion

        #region POST: Create
        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name, Selected_Groups, Selected_Roles, Email, Password")] CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var IsEmailUnique = _context.Users.Where(c => c.Email == model.Email).Any();
                if (IsEmailUnique)
                {
                    ViewData["Message"] = "البريد الالكتروني مسجل بالفعل";
                    return View(model);
                }

                User user = new User() 
                { 
                    Name = model.Name,
                    Email = model.Email, 
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password) 
                };
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
        #endregion

        #region GET: Edit
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

            return View(new CreateUserVM() { Id = id.Value, Name = user.Name, Email = user.Email});
        }
        #endregion
        
        #region POST: Edit
        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name, Selected_Groups, Selected_Roles, Email, Password")] CreateUserVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var updatedUser = new User() { Id = id, Name = model.Name, Email = model.Email };
                    if(model.Password is not null)
                    {
                        updatedUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    }
                    _context.Update(updatedUser);

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
        #endregion
        
        #region GET: Delete
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
        #endregion
        
        #region POST: Delete
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
        #endregion

        private bool UserExists(int id)
        {
          return _context.Users.Any(e => e.Id == id);
        }

        #region GET: PendingUser
        public async Task<IActionResult> PendingUsers()
        {
           return View();
        }
        #endregion

        #region GET: PendingUsersList 
        public async Task<IActionResult> PendingUsersList()
        {
            List<int?> usersId = await _context.UserRoleMappings.Where(c => c.RoleId == 4).Select(c => c.UserId).ToListAsync();
            var users = await _context.Users.Where(c => usersId.Contains(c.Id)).Select(c => new { c.Id, c.Name, c.Email }).ToListAsync();

            return Json(new
            {
                data = users
            });
        }
        #endregion

        #region POST: AcceptUsers
        public async Task<IActionResult> AcceptUsers(IFormCollection form)
        {
            List<int> usersIds = new();
            var usersIds_str = form["users"].ToString().Split(',').ToList();

            if (usersIds_str[0] == ""){ return RedirectToAction("BatchOrder");}

            foreach (var id in usersIds_str)
            {
                usersIds.Add(Convert.ToInt32(id));
            }
            var roleId = Convert.ToInt32(Request.Form["role"]);

            foreach(var userId in usersIds)
            {
                _context.UserRoleMappings.RemoveRange(_context.UserRoleMappings.Where(c => c.UserId == userId));
                _context.Add(new UserRoleMapping() { UserId = userId, RoleId = roleId });
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingUsers));
        }
        #endregion
    }
}
