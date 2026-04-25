using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.domain.entity;
using backend.context.booking.domain.repo;
using backend.context.booking.domain.vo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.booking.infrastructure.persistence;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    // Một slot 30 phút — chỉ chặn booking trùng đúng slot đó
    private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

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
        var targetId = new backend.context.identity.domain.vo.UserIdVO(customerId);
        return await _context.Bookings
            .Where(b => b.CustomerId == targetId)
            .ToListAsync();
    }

    /// <summary>
    /// FIX TIMEZONE: nhận ngày local của client (DateTimeKind.Unspecified hoặc Local),
    /// quy đổi về UTC một cách tường minh dựa trên múi giờ Việt Nam (UTC+7).
    /// </summary>
    public async Task<IEnumerable<Booking>> GetByDateAsync(DateTime date)
    {
        // Chuẩn hoá về UTC+7 trước, sau đó đổi sang UTC để so sánh với DB
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        // date.Date là đầu ngày local (00:00:00)
        var startOfDayLocal = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startOfDayLocal, vnTimeZone);
        var endUtc   = startUtc.AddDays(1);

        return await _context.Bookings
            .Where(b => b.BookingTime >= startUtc && b.BookingTime < endUtc)
            .OrderBy(b => b.BookingTime)
            .ToListAsync();
    }

    /// <summary>
    /// Kiểm tra slot trùng: tìm bất kỳ booking nào Pending/Confirmed
    /// rơi vào đúng cửa sổ [bookingTimeUtc, bookingTimeUtc + 30 phút).
    /// </summary>
    public async Task<bool> HasConflictAsync(DateTime bookingTimeUtc)
    {
        var slotEnd = bookingTimeUtc.Add(SlotDuration);

        return await _context.Bookings
            .Where(b => b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
            .AnyAsync(b => b.BookingTime >= bookingTimeUtc && b.BookingTime < slotEnd);
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
