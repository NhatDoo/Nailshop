using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.context.todo.domain.entity;

namespace backend.context.todo.domain.repo;

public interface ITodoRepository
{
    Task AddAsync(Todo todo);
    Task<Todo?> GetByIdAsync(Guid id);
    Task<IEnumerable<Todo>> GetTodosByUserIdAsync(string userId);
    Task UpdateAsync(Todo todo);
    Task DeleteAsync(Todo todo);
}
