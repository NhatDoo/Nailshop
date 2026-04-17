using System;
using backend.context.common.application;

namespace backend.context.booking.application.commands;

public record CreateBookingCommand(
    Guid CustomerId,
    string ServiceName,
    decimal Price,
    DateTime BookingTime,
    string? Note
) : ICommand<Guid>;
