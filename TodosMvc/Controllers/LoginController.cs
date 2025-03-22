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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVm model)
        {
            if (ModelState.IsValid) {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                var passwordHash = BCrypt.Net.BCrypt.Verify(model.Password, user.Passwordhash);

                if (user == null || !passwordHash) {
                    ViewBag.Error("User or Password are incorrect");
                    return View();
                }

                var token = await _authService.GenerateJwtToken(model) ?? string.Empty;

                if (string.IsNullOrEmpty(token)) {
                    ViewBag.Error("User or Password are incorrect");
                    return View();
                }
                
                return RedirectToAction("Index", "Todos");
            }
            return View();
        }

        public IActionResult Register()
        {
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

        private void SetJwtCookie(object token)
        {
            throw new NotImplementedException();
        }

        private object GenerateJwtToken(string username)
        {
            throw new NotImplementedException();
        }

    }
}
