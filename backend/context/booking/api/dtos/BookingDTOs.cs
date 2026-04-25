using System;

namespace backend.context.booking.api.dtos;

/// <summary>
/// Client chỉ được gửi ServiceName + BookingTime + Note.
/// Price KHÔNG nhận từ client — backend tự tra bảng dịch vụ.
/// </summary>
public record CreateBookingRequest(
    Guid ServiceId,
    DateTime BookingTime,
    string? Note
);
