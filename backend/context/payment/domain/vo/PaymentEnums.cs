namespace backend.context.payment.domain.vo;

public enum PaymentStatus
{
    Pending,   // Chờ thanh toán
    Success,   // Thành công
    Failed,    // Thất bại
    Refunded   // Đã hoàn tiền
}

public enum PaymentMethod
{
    VNPay,
    Cash,
    BankTransfer
}
