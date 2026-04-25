using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.booking.domain.entity;
using backend.context.booking.domain.repo;
using backend.context.identity.domain.vo;
using backend.context.nailservice.domain.repo;

namespace backend.context.booking.application.commands;

public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly INailServiceRepository _nailServiceRepository;

    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        INailServiceRepository nailServiceRepository)
    {
        _bookingRepository = bookingRepository;
        _nailServiceRepository = nailServiceRepository;
    }

    public async Task<Guid> HandleAsync(CreateBookingCommand command)
    {
        // 1. Tra giá từ DB — không tin Client
        var service = await _nailServiceRepository.GetByIdAsync(command.ServiceId)
            ?? throw new ArgumentException($"Dịch vụ không tồn tại: {command.ServiceId}");

        if (!service.IsActive)
            throw new ArgumentException("Dịch vụ này hiện không khả dụng.");

        // 2. Tính giá thực (có thể áp khuyến mãi sau)
        var price = service.Price;
        var serviceName = service.Name;

        // 3. Kiểm tra slot trùng trước khi tạo booking
        var bookingTimeUtc = command.BookingTime.Kind == DateTimeKind.Utc
            ? command.BookingTime
            : command.BookingTime.ToUniversalTime();

        var hasConflict = await _bookingRepository.HasConflictAsync(bookingTimeUtc);
        if (hasConflict)
            throw new InvalidOperationException(
                $"Khung giờ {command.BookingTime:HH:mm} đã được đặt. Vui lòng chọn giờ khác.");

        // 4. Khởi tạo đối tượng Domain
        var booking = Booking.Create(
            new UserIdVO(command.CustomerId),
            serviceName,
            price,
            command.BookingTime,
            command.Note
        );

        // 5. Lưu vào CSDL
        await _bookingRepository.AddAsync(booking);

        return booking.Id;
    }
}
