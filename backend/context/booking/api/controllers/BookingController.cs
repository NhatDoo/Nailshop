using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.booking.api.controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly CreateBookingCommandHandler _createBookingHandler;
    private readonly GetMyBookingsQueryHandler _getMyBookingsHandler;
    private readonly GetAllBookingsForDateQueryHandler _getByDateHandler;

    public BookingController(
        CreateBookingCommandHandler createBookingHandler,
        GetMyBookingsQueryHandler getMyBookingsHandler,
        GetAllBookingsForDateQueryHandler getByDateHandler)
    {
        _createBookingHandler = createBookingHandler;
        _getMyBookingsHandler = getMyBookingsHandler;
        _getByDateHandler = getByDateHandler;
    }

    /// <summary>GET /api/booking/available-slots?date=2026-04-20 - Public: xem giờ còn trống</summary>
    [AllowAnonymous]
    [HttpGet("available-slots")]
    public IActionResult GetAvailableSlots([FromQuery] string? date)
    {
        // Parse ngày, mặc định hôm nay
        var targetDate = DateTime.Today;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsed))
            targetDate = parsed.Date;

        // Khung giờ hoạt động: 8:00 - 18:00, mỗi slot 30 phút
        var allSlots = Enumerable.Range(0, 20)
            .Select(i => targetDate.AddHours(8).AddMinutes(i * 30))
            .ToList();

        return Ok(allSlots.Select(s => new
        {
            dateTime = s,
            display = s.ToString("HH:mm"),
            isoString = s.ToString("o")
        }));
    }

    /// <summary>GET /api/booking/my - Lấy booking của tôi (cần login)</summary>
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        // Lấy UserId từ Claims trong Token JWT
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var query = new GetMyBookingsQuery(Guid.Parse(userIdStr));
        var result = await _getMyBookingsHandler.HandleAsync(query);
        return Ok(result);
    }

    /// <summary>GET /api/booking/date/2026-04-20 - Admin: xem mọi booking trong ngày</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetBookingsByDate(string date)
    {
        if (!DateTime.TryParse(date, out var parsedDate))
            return BadRequest("Định dạng ngày không hợp lệ (yyyy-MM-dd)");

        var query = new GetAllBookingsForDateQuery(parsedDate);
        var result = await _getByDateHandler.HandleAsync(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(backend.context.booking.api.dtos.CreateBookingRequest request)
    {
        try
        {
            // Lấy UserId từ Token an toàn
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var command = new CreateBookingCommand(
                Guid.Parse(userIdStr),
                request.ServiceName,
                request.Price,
                request.BookingTime,
                request.Note
            );

            var bookingId = await _createBookingHandler.HandleAsync(command);
            return Ok(new { Message = "Đặt lịch thành công!", BookingId = bookingId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Có lỗi xảy ra: " + ex.Message);
        }
    }
}
