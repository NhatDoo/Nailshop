using System;
using System.Collections.Generic;
using backend.context.common.application;

namespace backend.context.todo.application.queries;

public record TodoResponse(
    Guid Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    string UserId,
    IEnumerable<Guid> BookingIds,
    DateTime CreatedAt
);

public record GetMyTodosQuery(string UserId) : IQuery<IEnumerable<TodoResponse>>;

public record GetTodoByIdQuery(Guid Id, string UserId) : IQuery<TodoResponse?>;
