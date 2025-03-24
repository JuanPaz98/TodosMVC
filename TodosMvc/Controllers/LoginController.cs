using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;
using TodosMvc.Models.ViewModels;
using TodosMvc.Services.Interfaces;

namespace TodosMvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly TodosContext _context;
        private readonly IAuthService _authService;

        public LoginController(TodosContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Todos");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Passwordhash))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var token = _authService.GenerateJwtToken(model);

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Failed to generate authentication token";
                return View(model);
            }

            _authService.StoreJwtInCookie(token);

            return RedirectToAction("Index", "Todos");
        }

        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Todos");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var userExist = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (userExist)
                {
                    ViewBag.Error = "User already exist";
                    return View();
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Passwordhash = passwordHash,
                    Createdat = DateTime.Now
                };

                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            _authService.LogoutAsync();
            return RedirectToAction("Index");
        }
    }
}
