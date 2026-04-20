using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.context.booking.domain.entity;

namespace backend.context.booking.domain.repo;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Booking>> GetByDateAsync(DateTime date);
    Task UpdateAsync(Booking booking);
}
