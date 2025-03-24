using TodosMvc.Models;
using TodosMvc.Models.ViewModels;

namespace TodosMvc.Services.Interfaces
{
    public interface ITodosService
    {
        public Task<IEnumerable<Todo>> GetTodosByUserId();
        public Task<bool> Create(TodoVM model);
    }
}
