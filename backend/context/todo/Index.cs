using backend.context.todo.application.commands;
using backend.context.todo.application.queries;
using backend.context.todo.domain.repo;
using backend.context.todo.infrastructure.persistence;
using backend.context.todo.infrastructure.services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.context.todo;

public static class TodoModule
{
    public static IServiceCollection AddTodoModule(this IServiceCollection services)
    {
        // Repository
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<ITodoOverdueRepository, TodoOverdueRepository>();

        // Command Handlers
        services.AddScoped<CreateTodoCommandHandler>();
        services.AddScoped<UpdateTodoCommandHandler>();
        services.AddScoped<ChangeTodoStatusCommandHandler>();
        services.AddScoped<AddBookingToTodoCommandHandler>();
        services.AddScoped<DeleteTodoCommandHandler>();

        // Query Handlers
        services.AddScoped<GetMyTodosQueryHandler>();
        services.AddScoped<GetTodoByIdQueryHandler>();

        // Overdue Queue + Background Worker
        // Queue là Singleton để chia sẻ state giữa Scanner và Processor threads
        services.AddSingleton<TodoOverdueQueue>();
        services.AddHostedService<TodoOverdueWorker>();

        return services;
    }
}
