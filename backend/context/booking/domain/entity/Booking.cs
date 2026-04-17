using System;
using backend.context.common.domain;
using backend.context.booking.domain.vo;
using backend.context.booking.domain.events;
using backend.context.identity.domain.vo;

namespace backend.context.booking.domain.entity;

public class Booking : AggregateRoot
{
    public Guid Id { get; private set; }
    public UserIdVO CustomerId { get; private set; }
    public string ServiceName { get; private set; }
    public decimal Price { get; private set; }
    public DateTime BookingTime { get; private set; }
    public BookingStatus Status { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Booking(Guid id, UserIdVO customerId, string serviceName, decimal price, DateTime bookingTime, string? note)
    {
        Id = id;
        CustomerId = customerId;
        ServiceName = serviceName;
        Price = price;
        BookingTime = bookingTime;
        Status = BookingStatus.Pending;
        Note = note;
        CreatedAt = DateTime.UtcNow;
    }

    private Booking() { } // Required for EF Core

    public static Booking Create(UserIdVO customerId, string serviceName, decimal price, DateTime bookingTime, string? note)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new ArgumentException("Tên dịch vụ không được để trống.");
        }

        if (serviceName.Length > 200)
        {
            throw new ArgumentException("Tên dịch vụ không được dài quá 200 ký tự.");
        }

        if (bookingTime <= DateTime.UtcNow)
        {
            throw new ArgumentException("Thời gian đặt lịch phải ở trong tương lai.");
        }

        if (price < 0)
        {
            throw new ArgumentException("Giá dịch vụ không được âm.");
        }

        var booking = new Booking(Guid.NewGuid(), customerId, serviceName, price, bookingTime, note);

        // Bắn sự kiện Domain Event
        booking.AddDomainEvent(new BookingCreatedEvent(
            booking.Id, 
            customerId.Value, 
            serviceName, 
            bookingTime));

        return booking;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new Exception("Chỉ có thể xác nhận lịch hẹn đang ở trạng thái chờ duyệt.");
        }
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Completed)
        {
            throw new Exception("Không thể hủy lịch hẹn đã hoàn thành.");
        }
        Status = BookingStatus.Cancelled;
    }
}
