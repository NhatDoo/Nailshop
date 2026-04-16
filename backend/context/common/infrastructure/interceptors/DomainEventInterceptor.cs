using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using backend.context.common.domain;
using backend.context.identity.application.events;
using backend.context.identity.domain.events;

namespace backend.context.common.infrastructure.interceptors;

public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly UserRegisteredEventHandler _userRegisteredEventHandler;

    public DomainEventInterceptor(UserRegisteredEventHandler userRegisteredEventHandler)
    {
        _userRegisteredEventHandler = userRegisteredEventHandler;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context == null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        // Lấy tất cả các Entity kế thừa AggregateRoot và có Event
        var entities = eventData.Context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        // Dispatch Event (Trong thực tế nên dùng Mediator, ở đây mình gọi thẳng hoặc dùng Service)
        foreach (var entity in entities)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                if (domainEvent is UserRegisteredEvent userEvent)
                {
                    await _userRegisteredEventHandler.HandleAsync(userEvent);
                }
            }
            entity.ClearDomainEvents();
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
