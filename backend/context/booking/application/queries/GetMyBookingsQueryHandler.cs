using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.booking.domain.repo;

namespace backend.context.booking.application.queries;

public class GetMyBookingsQueryHandler : IQueryHandler<GetMyBookingsQuery, IEnumerable<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;

    public GetMyBookingsQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<IEnumerable<BookingResponse>> HandleAsync(GetMyBookingsQuery query)
    {
        var bookings = await _bookingRepository.GetByCustomerIdAsync(query.CustomerId);

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
