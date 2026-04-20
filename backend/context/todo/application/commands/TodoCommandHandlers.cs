using System;
using System.Threading.Tasks;
using backend.context.identity.domain.vo;
using backend.context.todo.domain.entity;
using backend.context.todo.domain.repo;
using backend.context.todo.domain.vo;
using backend.context.common.application;

namespace backend.context.todo.application.commands;

public class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, Guid>
{
    private readonly ITodoRepository _repository;

    public CreateTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> HandleAsync(CreateTodoCommand command)
    {
        if (!Enum.TryParse<TodoPriority>(command.Priority, true, out var priority))
            throw new ArgumentException("Mức độ ưu tiên không hợp lệ.");

        var todo = Todo.Create(command.Title, command.Description, priority, command.DueDate, new UserIdVO(Guid.Parse(command.UserId)));
        
        await _repository.AddAsync(todo);
        return todo.Id;
    }
}

public class UpdateTodoCommandHandler : ICommandHandler<UpdateTodoCommand, bool>
{
    private readonly ITodoRepository _repository;

    public UpdateTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(UpdateTodoCommand command)
    {
        var todo = await _repository.GetByIdAsync(command.Id) ?? throw new Exception("Không tìm thấy Todo.");
        
        if (todo.UserId.Value.ToString() != command.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Todo này.");

        if (!Enum.TryParse<TodoPriority>(command.Priority, true, out var priority))
            throw new ArgumentException("Mức độ ưu tiên không hợp lệ.");

        todo.Update(command.Title, command.Description, command.DueDate, priority);
        
        await _repository.UpdateAsync(todo);
        return true;
    }
}

public class ChangeTodoStatusCommandHandler : ICommandHandler<ChangeTodoStatusCommand, bool>
{
    private readonly ITodoRepository _repository;

    public ChangeTodoStatusCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(ChangeTodoStatusCommand command)
    {
        var todo = await _repository.GetByIdAsync(command.Id) ?? throw new Exception("Không tìm thấy Todo.");
        
        if (todo.UserId.Value.ToString() != command.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Todo này.");

        if (!Enum.TryParse<TodoStatus>(command.Status, true, out var status))
            throw new ArgumentException("Trạng thái không hợp lệ. Chỉ chấp nhận: Todo, InProgress, Done");

        todo.ChangeStatus(status);
        
        await _repository.UpdateAsync(todo);
        return true;
    }
}

public class AddBookingToTodoCommandHandler : ICommandHandler<AddBookingToTodoCommand, bool>
{
    private readonly ITodoRepository _repository;

    public AddBookingToTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(AddBookingToTodoCommand command)
    {
        var todo = await _repository.GetByIdAsync(command.TodoId) ?? throw new Exception("Không tìm thấy Todo.");
        
        if (todo.UserId.Value.ToString() != command.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền sửa Todo này.");

        todo.AddBooking(command.BookingId);
        
        await _repository.UpdateAsync(todo);
        return true;
    }
}

public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand, bool>
{
    private readonly ITodoRepository _repository;

    public DeleteTodoCommandHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteTodoCommand command)
    {
        var todo = await _repository.GetByIdAsync(command.Id) ?? throw new Exception("Không tìm thấy Todo.");
        
        if (todo.UserId.Value.ToString() != command.UserId)
            throw new UnauthorizedAccessException("Bạn không có quyền xóa Todo này.");

        await _repository.DeleteAsync(todo);
        return true;
    }
}
