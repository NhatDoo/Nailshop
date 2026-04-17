using System;
using System.Collections.Generic;
using backend.context.common.application;

namespace backend.context.booking.application.queries;

public record GetMyBookingsQuery(Guid CustomerId) : IQuery<IEnumerable<BookingResponse>>;

public record BookingResponse(
    Guid Id,
    string ServiceName,
    decimal Price,
    DateTime BookingTime,
    string Status,
    string? Note
);
