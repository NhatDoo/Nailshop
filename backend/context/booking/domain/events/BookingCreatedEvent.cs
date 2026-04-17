using System;
using backend.context.common.domain;

namespace backend.context.booking.domain.events;

public record BookingCreatedEvent(
    Guid BookingId,
    Guid CustomerId,
    string ServiceName,
    DateTime BookingTime
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
