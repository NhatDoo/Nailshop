using System;
using System.Collections.Concurrent;

namespace backend.context.todo.infrastructure.services;

public enum OverdueTaskType
{
    Todo,
    Booking
}

public record OverdueTask(Guid Id, OverdueTaskType Type);

/// <summary>
/// Thread-safe queue chứa thông tin các thực thể (Todo/Booking) cần được cập nhật trạng thái.
/// </summary>
public sealed class TodoOverdueQueue
{
    private readonly ConcurrentQueue<OverdueTask> _queue = new();

    public void Enqueue(Guid id, OverdueTaskType type) => _queue.Enqueue(new OverdueTask(id, type));

    public bool TryDequeue(out OverdueTask task) => _queue.TryDequeue(out task);

    public int Count => _queue.Count;
}
