using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;

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
            ViewData["CreateTodo"] = new SelectList(_context.Todos, "Id", "Title");
             
            return View();
        }

        //[HttpPost]
        //public IActionResult Create()
        //{
        //    ViewData["CreateTodo"] = new SelectList(_context.Todos, "Id", "Title");

        //    return View();
        //}
    }
}
