using System;
using backend.context.common.application;

namespace backend.context.booking.application.commands;

/// <summary>
/// Price bị loại bỏ — handler tự look-up giá từ INailServiceRepository.
/// </summary>
public record CreateBookingCommand(
    Guid CustomerId,
    Guid ServiceId,
    DateTime BookingTime,
    string? Note
) : ICommand<Guid>;
