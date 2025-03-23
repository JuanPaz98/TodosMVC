using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;
using TodosMvc.Models.ViewModels;

namespace TodosMvc.Controllers
{
    [Authorize]
    public class TodosController : Controller
    {
        private readonly TodosContext _context;

        public TodosController(TodosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _context.Todos.ToListAsync();

            return View(todos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoVM model)
        {
            if (ModelState.IsValid)
            {
                var todo = new Todo
                {
                    Title = model.Title,
                    Description = model.Description,
                    Duedate = model.DueDate,
                    Createdat = DateTime.Now,
                    Status = model.Status.ToString(),
                    Userid = 3
                };
                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    };

};


