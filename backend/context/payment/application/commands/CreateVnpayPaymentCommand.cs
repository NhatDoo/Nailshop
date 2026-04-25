using System;

namespace backend.context.payment.application.commands;

/// <summary>
/// Amount bị loại bỏ — handler tự tra BookingRepository.
/// </summary>
public record CreateVnpayPaymentCommand(
    Guid BookingId,
    string OrderInfo,
    string ReturnUrl,
    string IpAddress,
    string? BankCode = null
);

public record CreateVnpayPaymentResult(
    Guid PaymentId,
    string PaymentUrl
);
