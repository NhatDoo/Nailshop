using System;

namespace backend.context.payment.application.commands;

public record CreateVnpayPaymentCommand(
    Guid BookingId,
    long Amount,  // Số tiền chuẩn (VND) chưa nhân 100
    string OrderInfo,
    string ReturnUrl,
    string IpAddress,
    string? BankCode = null
);

public record CreateVnpayPaymentResult(
    Guid PaymentId,
    string PaymentUrl
);
