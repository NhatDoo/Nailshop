using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.domain.entity;
using backend.context.booking.domain.repo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.booking.infrastructure.persistence;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<IEnumerable<Booking>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Bookings
            .Where(b => b.CustomerId.Value == customerId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
