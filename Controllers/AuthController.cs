using Meshkah.Models;
using Meshkah.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Meshkah.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly MeshkahContext _context;

        public AuthController(MeshkahContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email, Password")] AuthVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(c => c.Email == model.Email).FirstOrDefault();
                if (user == null) 
                {
                    ViewData["Message"] = "البريد الالكتروني غير صحيح";
                    return View(model); 
                }


                var claims = new List<Claim>
                {
                    new Claim("Name", user.Name),
                    new Claim("Email", user.Email),
                    new Claim("UserId", user.Id.ToString())
                };

                var roles = await _context.UserRoleMappings.Where(c => c.UserId == user.Id).Select(c => c.Role.Name).ToListAsync();
                foreach(var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { };
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                Thread.CurrentPrincipal = claimsPrincipal;

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties
                );

                if(roles.Contains("Waiting"))
                {
                    ViewData["Message"] = "نعتذر";
                    return RedirectToAction(nameof(Waiting));
                }
                if(roles.Contains("Student"))
                {
                    ViewData["Message"] = "";
                    return RedirectToAction("Index", "Student");
                }
                if(roles.Contains("Admin") || roles.Contains("Supervisor"))
                {
                    ViewData["Message"] = "";
                    return RedirectToAction(nameof(Index), "Home");
                }
                return RedirectToAction(nameof(Waiting));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Name, Email, Password")] AuthVM model)
        {
            if (ModelState.IsValid)
            {
               var IsEmailUnique =  _context.Users.Where(c => c.Email == model.Email).Any();
                if (IsEmailUnique)
                {
                    ViewData["Message"] = "البريد الالكتروني مسجل بالفعل";
                    return View(model);
                }

                User user = new () 
                { 
                    Email = model.Email, 
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password) , 
                    Name = model.Name , 
                    IsActive = false
                };
                _context.Add(user);
                await _context.SaveChangesAsync();
                _context.Add(new UserRoleMapping() { UserId = user.Id, RoleId = 4 });
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                    {
                        new Claim("Name", user.Name),
                        new Claim("Email", user.Email),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Role, "Waiting")
                    };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties {};
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                Thread.CurrentPrincipal = claimsPrincipal;

                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties
                );

                return RedirectToAction(nameof(Waiting));
            }
            return View(model);
        }

        [Authorize(Roles = "Waiting")] 
        public IActionResult Waiting()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
