using System;

namespace backend.context.payment.api.dtos;

/// <summary>
/// Amount KHÔNG được nhận từ client.
/// Backend tự tra Booking để xác định số tiền.
/// </summary>
public record CreatePaymentRequest(
    Guid BookingId,
    string ReturnUrl,
    string? BankCode = null
);
