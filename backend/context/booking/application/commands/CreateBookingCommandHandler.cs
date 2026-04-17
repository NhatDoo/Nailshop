using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.booking.domain.entity;
using backend.context.booking.domain.repo;
using backend.context.identity.domain.vo;

namespace backend.context.booking.application.commands;

public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookingRepository;

    public CreateBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Guid> HandleAsync(CreateBookingCommand command)
    {
        // 1. Khởi tạo đối tượng Domain
        var booking = Booking.Create(
            new UserIdVO(command.CustomerId),
            command.ServiceName,
            command.Price,
            command.BookingTime,
            command.Note
        );

        // 2. Lưu vào CSDL
        await _bookingRepository.AddAsync(booking);

        return booking.Id;
    }
}
