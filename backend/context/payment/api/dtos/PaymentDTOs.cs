using System;

namespace backend.context.payment.api.dtos;

public record CreatePaymentRequest(
    Guid BookingId,
    long Amount,
    string ReturnUrl,
    string? BankCode = null
);
