using System.Collections.Generic;

namespace backend.context.payment.application.commands;

public record ProcessVnpayCallbackCommand(
    IDictionary<string, string> VnpayData
);

public record ProcessVnpayCallbackResult(
    bool IsSuccess,
    string Message
);
