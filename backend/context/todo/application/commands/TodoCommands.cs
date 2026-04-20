using System;
using System.Collections.Generic;
using backend.context.common.application;

namespace backend.context.todo.application.commands;

public record CreateTodoCommand(
    string Title,
    string? Description,
    string Priority, // "Low" | "Medium" | "High"
    DateTime? DueDate,
    string UserId
) : ICommand<Guid>;

public record UpdateTodoCommand(
    Guid Id,
    string Title,
    string? Description,
    string Priority,
    DateTime? DueDate,
    string UserId
) : ICommand<bool>;

public record ChangeTodoStatusCommand(
    Guid Id,
    string Status, // "Todo" | "InProgress" | "Done"
    string UserId
) : ICommand<bool>;

public record AddBookingToTodoCommand(
    Guid TodoId,
    Guid BookingId,
    string UserId
) : ICommand<bool>;

public record DeleteTodoCommand(
    Guid Id,
    string UserId
) : ICommand<bool>;
