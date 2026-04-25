using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using backend.context.common.domain;
using backend.context.identity.application.events;
using backend.context.identity.domain.events;

namespace backend.context.common.infrastructure.interceptors;

/// <summary>
/// Domain Event Dispatcher.
/// 
/// QUAN TRỌNG (Medium fix):
/// Chúng ta chuyển từ SavingChangesAsync (trước commit) sang SavedChangesAsync (sau commit)
/// để đảm bảo side-effect (email, notification,...) chỉ xảy ra khi data đã thực sự được lưu.
/// 
/// Trade-off: nếu handler bắn event thất bại sau khi đã commit, event đó có thể bị mất.
/// Giải pháp dài hạn là Outbox Pattern (lưu event vào DB trong cùng transaction).
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly UserRegisteredEventHandler _userRegisteredEventHandler;
    private readonly backend.context.booking.application.events.BookingCreatedEventHandler _bookingCreatedEventHandler;

    // Lưu tạm events trước SaveChanges để dispatch sau khi commit
    private List<(AggregateRoot Entity, IDomainEvent Event)> _pendingEvents = new();

    public DomainEventInterceptor(
        UserRegisteredEventHandler userRegisteredEventHandler,
        backend.context.booking.application.events.BookingCreatedEventHandler bookingCreatedEventHandler)
    {
        _userRegisteredEventHandler = userRegisteredEventHandler;
        _bookingCreatedEventHandler = bookingCreatedEventHandler;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            // Thu thập events TRƯỚC khi save (để clear khỏi entity)
            _pendingEvents = eventData.Context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents.Select(ev => (e.Entity, ev)))
                .ToList();

            // Clear events trên entity TRƯỚC khi save để tránh EF track thêm
            foreach (var (entity, _) in _pendingEvents)
                entity.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        // Dispatch SAU KHI commit thành công — tránh side effect khi transaction fail
        foreach (var (_, domainEvent) in _pendingEvents)
        {
            if (domainEvent is UserRegisteredEvent userEvent)
                await _userRegisteredEventHandler.HandleAsync(userEvent);
            else if (domainEvent is backend.context.booking.domain.events.BookingCreatedEvent bookingEvent)
                await _bookingCreatedEventHandler.HandleAsync(bookingEvent);
        }

        _pendingEvents.Clear();

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
