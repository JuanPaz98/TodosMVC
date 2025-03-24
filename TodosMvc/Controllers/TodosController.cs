using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;
using TodosMvc.Models.ViewModels;
using TodosMvc.Services.Interfaces;

namespace TodosMvc.Controllers
{
    [Authorize]
    public class TodosController : Controller
    {
        private readonly TodosContext _context;
        private readonly ITodosService _todosService;

        public TodosController(TodosContext context, ITodosService todosService)
        {
            _context = context;
            _todosService = todosService;
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _todosService.GetTodosByUserId();

            return View(todos);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> CompletedTodos()
        {
            var todos = await _context.Todos.ToListAsync();

            return View(todos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodoVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _todosService.Create(model);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTodo(Todo model)
        {
            var todo = await _context.Todos.FindAsync(model.Todoid);
            if (todo == null)
            {
                return NotFound();
            }

            todo.Title = model.Title;
            todo.Description = model.Description;
            todo.Duedate = model.Duedate;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    };

};


