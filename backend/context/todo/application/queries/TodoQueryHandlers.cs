using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.todo.domain.repo;

namespace backend.context.todo.application.queries;

public class GetMyTodosQueryHandler : IQueryHandler<GetMyTodosQuery, IEnumerable<TodoResponse>>
{
    private readonly ITodoRepository _repository;

    public GetMyTodosQueryHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponse>> HandleAsync(GetMyTodosQuery query)
    {
        var todos = await _repository.GetTodosByUserIdAsync(query.UserId);

        return todos.Select(t => new TodoResponse(
            t.Id,
            t.Title,
            t.Description,
            t.Status.ToString(),
            t.Priority.ToString(),
            t.DueDate,
            t.UserId.Value.ToString(),
            t.BookingIds,
            t.CreatedAt
        ));
    }
}

public class GetTodoByIdQueryHandler : IQueryHandler<GetTodoByIdQuery, TodoResponse?>
{
    private readonly ITodoRepository _repository;

    public GetTodoByIdQueryHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoResponse?> HandleAsync(GetTodoByIdQuery query)
    {
        var t = await _repository.GetByIdAsync(query.Id);

        if (t == null || t.UserId.Value.ToString() != query.UserId)
            return null;

        return new TodoResponse(
            t.Id,
            t.Title,
            t.Description,
            t.Status.ToString(),
            t.Priority.ToString(),
            t.DueDate,
            t.UserId.Value.ToString(),
            t.BookingIds,
            t.CreatedAt
        );
    }
}
