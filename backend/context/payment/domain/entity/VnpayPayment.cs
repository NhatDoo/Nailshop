using System;
using backend.context.payment.domain.vo;

namespace backend.context.payment.domain.entity;

/// <summary>
/// Kế thừa Payment, bổ sung toàn bộ tham số theo đặc tả VNPAY.
/// Tham chiếu: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/
/// </summary>
public class VnpayPayment : Payment
{
    // Tham số VNPAY bắt buộc
    public string TxnRef { get; private set; }        // Mã giao dịch duy nhất (vnp_TxnRef)
    public string OrderInfo { get; private set; }      // Mô tả đơn hàng (vnp_OrderInfo)
    public string OrderType { get; private set; }      // Loại hàng hóa (vnp_OrderType)
    public string ReturnUrl { get; private set; }      // URL VNPAY callback về (vnp_ReturnUrl)
    public string IpAddress { get; private set; }      // IP người dùng (vnp_IpAddr)
    public string Locale { get; private set; }         // Ngôn ngữ: vn | en (vnp_Locale)

    // Tham số tùy chọn
    public string? BankCode { get; private set; }      // Mã ngân hàng (vnp_BankCode)

    // Dữ liệu phát sinh sau khi tạo
    public string? PaymentUrl { get; private set; }    // URL đầy đủ để redirect khách hàng
    public string? SecureHash { get; private set; }    // Chữ ký HMAC-SHA256 (vnp_SecureHash)
    public string? TransactionNo { get; private set; } // Mã giao dịch trên hệ thống VNPAY (sau callback)

    private VnpayPayment() { } // EF Core

    // Factory Method
    public static VnpayPayment Create(
        Guid bookingId,
        long amount,
        string orderInfo,
        string returnUrl,
        string ipAddress,
        string locale = "vn",
        string orderType = "other",
        string? bankCode = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền thanh toán phải lớn hơn 0.");

        if (string.IsNullOrWhiteSpace(orderInfo))
            throw new ArgumentException("Thông tin đơn hàng không được để trống.");

        if (string.IsNullOrWhiteSpace(returnUrl))
            throw new ArgumentException("URL callback không được để trống.");

        return new VnpayPayment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            Amount = amount,
            Currency = "VND",
            Status = PaymentStatus.Pending,
            Method = PaymentMethod.VNPay,
            CreatedAt = DateTime.UtcNow,
            TxnRef = DateTime.UtcNow.Ticks.ToString(), // Unique theo thời gian
            OrderInfo = orderInfo,
            OrderType = orderType,
            ReturnUrl = returnUrl,
            IpAddress = ipAddress,
            Locale = locale,
            BankCode = bankCode
        };
    }

    /// <summary>
    /// Gán URL thanh toán sau khi được VnpayService tạo ra.
    /// </summary>
    public void SetPaymentUrl(string paymentUrl, string secureHash)
    {
        PaymentUrl = paymentUrl;
        SecureHash = secureHash;
    }

    /// <summary>
    /// Xác nhận thanh toán thành công từ VNPAY Callback.
    /// </summary>
    public void ConfirmFromCallback(string transactionNo)
    {
        MarkSuccess();
        TransactionNo = transactionNo;
    }
}
