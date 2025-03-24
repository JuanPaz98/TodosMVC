using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodosMvc.Models;
using TodosMvc.Models.ViewModels;
using TodosMvc.Services.Interfaces;

namespace TodosMvc.Services
{
    public class TodosService : ITodosService
    {

        private readonly TodosContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodosService(TodosContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Create(TodoVM model)
        {
            var todo = new Todo
            {
                Title = model.Title,
                Description = model.Description,
                Duedate = model.DueDate,
                Createdat = DateTime.Now,
                Status = model.Status.ToString(),
                Userid = GetUserId()
            };
            _context.Todos.Add(todo);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Todo>> GetTodosByUserId()
        {
            var userId = GetUserId();

            return await _context.Todos.Where(t => t.Userid == userId).ToListAsync();
        }

        public async Task<bool> Update(Todo model)
        {
            var existingTodo = await _context.Todos.FindAsync(model.Todoid);
            if (existingTodo != null)
            {
                existingTodo.Title = model.Title;
                existingTodo.Description = model.Description;
                existingTodo.Duedate = model.Duedate;

                _context.Todos.Update(existingTodo);
            }
            
            return await _context.SaveChangesAsync() > 0;
        }

        private int GetUserId()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userClaim == null)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == userClaim);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return user.Id;
        }
    }
}
