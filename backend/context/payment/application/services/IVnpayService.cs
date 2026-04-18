using System.Collections.Generic;
using backend.context.payment.domain.entity;

namespace backend.context.payment.application.services;

public interface IVnpayService
{
    /// <summary>
    /// Tạo URL thanh toán VNPAY từ thông tin đơn hàng
    /// </summary>
    string CreatePaymentUrl(VnpayPayment payment);

    /// <summary>
    /// Xác thực chữ ký dữ liệu từ VNPAY gửi về qua Callback/IPN
    /// </summary>
    bool ValidateSignature(IDictionary<string, string> responseData, string secureHash);
}
