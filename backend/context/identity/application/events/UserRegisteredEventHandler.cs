using System;
using System.Threading.Tasks;
using backend.context.identity.domain.events;

namespace backend.context.identity.application.events;

public class UserRegisteredEventHandler
{
    public Task HandleAsync(UserRegisteredEvent domainEvent)
    {
        // Giả lập logic xử lý sự kiện (ví dụ: gửi mail chào mừng, log, v.v.)
        Console.WriteLine($"[Event Handler] Người dùng mới đã đăng ký: {domainEvent.Email.Value} (ID: {domainEvent.UserId})");
        
        return Task.CompletedTask;
    }
}
