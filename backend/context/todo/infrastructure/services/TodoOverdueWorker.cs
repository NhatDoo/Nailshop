using System;
using System.Threading;
using System.Threading.Tasks;
using backend.context.todo.infrastructure.persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace backend.context.todo.infrastructure.services;

public sealed class TodoOverdueWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TodoOverdueQueue _queue;
    private readonly ILogger<TodoOverdueWorker> _logger;

    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(30); // Tăng tần suất quét lên 30s
    private static readonly TimeSpan IdleDelay = TimeSpan.FromMilliseconds(500);

    public TodoOverdueWorker(
        IServiceScopeFactory scopeFactory,
        TodoOverdueQueue queue,
        ILogger<TodoOverdueWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[TodoOverdueWorker] Khởi động hệ thống quét tự động.");

        var scannerTask = RunScannerAsync(stoppingToken);
        var processorTask = RunProcessorAsync(stoppingToken);

        await Task.WhenAll(scannerTask, processorTask);
    }

    private async Task RunScannerAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                await using var scope = _scopeFactory.CreateAsyncScope();
                var repo = scope.ServiceProvider.GetRequiredService<ITodoOverdueRepository>();

                // 1. Quét Todos
                var overdueTodoIds = await repo.GetOverdueTodoIdsAsync(utcNow);
                foreach (var id in overdueTodoIds)
                    _queue.Enqueue(id, OverdueTaskType.Todo);

                // 2. Quét Bookings
                var overdueBookingIds = await repo.GetOverdueBookingIdsAsync(utcNow);
                foreach (var id in overdueBookingIds)
                    _queue.Enqueue(id, OverdueTaskType.Booking);

                if (overdueTodoIds.Count > 0 || overdueBookingIds.Count > 0)
                {
                    _logger.LogInformation(
                        "[Scanner] Đã tìm thấy {TodoCount} Todo và {BookingCount} Booking quá hạn.",
                        overdueTodoIds.Count, overdueBookingIds.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Scanner] Lỗi khi quét dữ liệu quá hạn.");
            }

            await Task.Delay(ScanInterval, ct);
        }
    }

    private async Task RunProcessorAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (_queue.TryDequeue(out var task))
            {
                try
                {
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ITodoOverdueRepository>();

                    if (task.Type == OverdueTaskType.Todo)
                    {
                        await repo.MarkAsDoneAsync(task.Id);
                        _logger.LogInformation("[Processor] Todo {Id} -> Done.", task.Id);
                    }
                    else if (task.Type == OverdueTaskType.Booking)
                    {
                        await repo.MarkBookingAsCompletedAsync(task.Id);
                        _logger.LogInformation("[Processor] Booking {Id} -> Completed.", task.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Processor] Lỗi khi xử lý task {Id} ({Type}).", task.Id, task.Type);
                }
            }
            else
            {
                await Task.Delay(IdleDelay, ct);
            }
        }
    }
}
