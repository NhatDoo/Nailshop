using System;
using System.Collections.Generic;
using backend.context.common.domain;
using backend.context.identity.domain.vo;
using backend.context.todo.domain.vo;

namespace backend.context.todo.domain.entity;

public class Todo : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TodoStatus Status { get; private set; }
    public TodoPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }
    public UserIdVO UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core 8 yêu cầu collection kiểu nguyên thủy phải là type List hoặc mảng mới map được
    // Ta set private set để tránh gán đè cả list.
    public List<Guid> BookingIds { get; private set; } = new();

    private Todo()
    {
        Title = null!;
        UserId = null!;
    } // EF Core

    public static Todo Create(
        string title, 
        string? description, 
        TodoPriority priority, 
        DateTime? dueDate, 
        UserIdVO userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề công việc không được để trống.");

        return new Todo
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Status = TodoStatus.Todo, // Mặc định khi tạo mới
            Priority = priority,
            DueDate = dueDate,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string? description, DateTime? dueDate, TodoPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tiêu đề không được để trống.");

        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(TodoStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddBooking(Guid bookingId)
    {
        if (!BookingIds.Contains(bookingId))
        {
            BookingIds.Add(bookingId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveBooking(Guid bookingId)
    {
        if (BookingIds.Contains(bookingId))
        {
            BookingIds.Remove(bookingId);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
