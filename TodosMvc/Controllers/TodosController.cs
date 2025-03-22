using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;
using TodosMvc.Models.ViewModels;

namespace TodosMvc.Controllers
{
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
        public IActionResult Create(TodoVM model)
        {
            if (ModelState.IsValid)
            {
                var todo = new Todo
                {
                    Title = model.Title,
                    Description = model.Description,
                    Duedate = model.DueDate,
                    Createdat = DateTime.Now,
                    Status = model.Status,
                    Userid = 3
                };
                _context.Todos.Add(todo);
                _context.SaveChanges();
            }

            return View(model);
        }
    }
}
