using System;

namespace backend.context.todo.api.dtos;

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
}

public class UpdateTodoRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
}

public class ChangeTodoStatusRequest
{
    public string Status { get; set; } = string.Empty; // "Todo", "InProgress", "Done"
}

public class AddTodoBookingRequest
{
    public Guid BookingId { get; set; }
}
