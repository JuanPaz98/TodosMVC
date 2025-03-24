using TodosMvc.Models;
using TodosMvc.Models.ViewModels;

namespace TodosMvc.Services.Interfaces
{
    public interface ITodosService
    {
        public Task<IEnumerable<Todo>> GetTodosByUserId();
        public Task<IEnumerable<Todo>> GetCompletedTodos();
        public Task<bool> Create(TodoVM model);
        public Task<bool> Update(Todo model);
        public Task<bool> UpdateStatus(int id);
        public Task<bool> Delete(int id);
    }
}
