using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.todo.domain.entity;
using backend.context.todo.domain.vo;
using backend.context.booking.domain.entity;
using backend.context.booking.domain.vo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.todo.infrastructure.persistence;

public interface ITodoOverdueRepository
{
    // Todos
    Task<IReadOnlyList<Guid>> GetOverdueTodoIdsAsync(DateTime utcNow);
    Task MarkAsDoneAsync(Guid id);

    // Bookings
    Task<IReadOnlyList<Guid>> GetOverdueBookingIdsAsync(DateTime utcNow);
    Task MarkBookingAsCompletedAsync(Guid id);
}

public class TodoOverdueRepository : ITodoOverdueRepository
{
    private readonly AppDbContext _context;

    public TodoOverdueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Guid>> GetOverdueTodoIdsAsync(DateTime utcNow)
    {
        return await _context.Todos
            .AsNoTracking()
            .Where(t => 
                t.DueDate.HasValue && 
                t.DueDate.Value < utcNow && 
                t.Status != TodoStatus.Done)
            .Select(t => t.Id)
            .ToListAsync();
    }

    public async Task MarkAsDoneAsync(Guid id)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.Status != TodoStatus.Done);
        
        if (todo == null) return;
        
        todo.ChangeStatus(TodoStatus.Done);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Guid>> GetOverdueBookingIdsAsync(DateTime utcNow)
    {
        // BookingTime đã qua và chưa Completed/Cancelled
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => 
                b.BookingTime < utcNow && 
                b.Status != BookingStatus.Completed && 
                b.Status != BookingStatus.Cancelled)
            .Select(b => b.Id)
            .ToListAsync();
    }

    public async Task MarkBookingAsCompletedAsync(Guid id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);
            
        if (booking == null) return;
        
        // Cập nhật trạng thái sang Completed
        booking.Complete(); 
        await _context.SaveChangesAsync();
    }
}
