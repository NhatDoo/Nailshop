namespace backend.context.booking.domain.vo;

public enum BookingStatus
{
    Pending,     // Chờ xác nhận
    Confirmed,   // Đã xác nhận
    Completed,   // Đã hoàn thành
    Cancelled    // Đã hủy
}
