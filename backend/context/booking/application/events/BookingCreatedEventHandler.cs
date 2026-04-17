using System.Threading.Tasks;
using backend.context.booking.domain.events;
using Microsoft.Extensions.Logging;

namespace backend.context.booking.application.events;

public class BookingCreatedEventHandler
{
    private readonly ILogger<BookingCreatedEventHandler> _logger;

    public BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(BookingCreatedEvent domainEvent)
    {
        // Giả lập logic xử lý: Ví dụ gửi thông báo cho thợ Nail
        _logger.LogInformation($"[EVENT] Lịch hẹn mới đã được tạo: {domainEvent.BookingId} cho dịch vụ {domainEvent.ServiceName} vào lúc {domainEvent.BookingTime}");
        
        return Task.CompletedTask;
    }
}
