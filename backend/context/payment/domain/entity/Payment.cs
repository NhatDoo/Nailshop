using System;
using backend.context.common.domain;
using backend.context.payment.domain.vo;

namespace backend.context.payment.domain.entity;

/// <summary>
/// Abstract Aggregate Root chứa thông tin chung của mọi phương thức thanh toán.
/// EF Core sẽ dùng TPH (Table Per Hierarchy) - một bảng duy nhất với cột Discriminator.
/// </summary>
public abstract class Payment : AggregateRoot
{
    public Guid Id { get; protected set; }
    public Guid BookingId { get; protected set; }   // Liên kết với Booking
    public long Amount { get; protected set; }       // VND * 100 theo chuẩn VNPAY
    public string Currency { get; protected set; }   // "VND"
    public PaymentStatus Status { get; protected set; }
    public PaymentMethod Method { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected Payment() { } // EF Core

    protected Payment(Guid bookingId, long amount, string currency, PaymentMethod method)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        Amount = amount;
        Currency = currency;
        Status = PaymentStatus.Pending;
        Method = method;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkSuccess()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Chỉ có thể xác nhận thanh toán đang ở trạng thái Pending.");
        Status = PaymentStatus.Success;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Chỉ có thể đánh dấu thất bại cho thanh toán đang Pending.");
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
