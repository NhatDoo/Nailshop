using System;

namespace backend.context.booking.api.dtos;

public record CreateBookingRequest(
    string ServiceName,
    decimal Price,
    DateTime BookingTime,
    string? Note
);
