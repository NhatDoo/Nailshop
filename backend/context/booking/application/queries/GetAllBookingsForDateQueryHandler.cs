using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.booking.domain.repo;
using backend.context.booking.domain.entity;

namespace backend.context.booking.application.queries;

public record GetAllBookingsForDateQuery(DateTime TargetDate) : IQuery<IEnumerable<BookingResponse>>;

public class GetAllBookingsForDateQueryHandler : IQueryHandler<GetAllBookingsForDateQuery, IEnumerable<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetAllBookingsForDateQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingResponse>> HandleAsync(GetAllBookingsForDateQuery query)
    {
        var bookings = await _bookingRepository.GetByDateAsync(query.TargetDate);

        return bookings.Select(b => new BookingResponse(
            b.Id,
            b.ServiceName,
            b.Price,
            b.BookingTime,
            b.Status.ToString(),
            b.Note
        ));
    }
}
