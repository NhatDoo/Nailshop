using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.todo.domain.entity;
using backend.context.todo.domain.repo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.todo.infrastructure.persistence;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Todo todo)
    {
        await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<IEnumerable<Todo>> GetTodosByUserIdAsync(string userId)
    {
        // Primitive Collection (BookingIds) EFCore 8 sẽ tự load
        return await _context.Todos
            .Where(t => t.UserId.Value.ToString() == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Todo todo)
    {
        _context.Todos.Update(todo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Todo todo)
    {
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
    }
}
