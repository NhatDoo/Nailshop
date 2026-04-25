using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using backend.context.booking.domain.repo;
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
    private readonly IBookingRepository _bookingRepository;

    public BookingController(
        CreateBookingCommandHandler createBookingHandler,
        GetMyBookingsQueryHandler getMyBookingsHandler,
        GetAllBookingsForDateQueryHandler getByDateHandler,
        IBookingRepository bookingRepository)
    {
        _createBookingHandler = createBookingHandler;
        _getMyBookingsHandler = getMyBookingsHandler;
        _getByDateHandler = getByDateHandler;
        _bookingRepository = bookingRepository;
    }

    /// <summary>
    /// GET /api/booking/available-slots?date=2026-04-20
    /// Public: trả về các khung giờ còn trống trong ngày (dựa trên DB thực tế).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] string? date)
    {
        // Parse ngày theo múi giờ VN, mặc định hôm nay
        var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var nowVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

        var targetDate = nowVn.Date;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsed))
            targetDate = parsed.Date;

        // Khung giờ hoạt động: 8:00 - 18:00, mỗi slot 30 phút → 20 slots
        var allSlots = Enumerable.Range(0, 20)
            .Select(i => targetDate.AddHours(8).AddMinutes(i * 30))
            .ToList();

        // Kiểm tra từng slot với DB
        var result = new List<object>();
        foreach (var slot in allSlots)
        {
            // Quy đổi slot local VN sang UTC để tra DB
            var slotUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(slot, DateTimeKind.Unspecified), vnTimeZone);

            var isBooked = await _bookingRepository.HasConflictAsync(slotUtc);
            result.Add(new
            {
                dateTime   = slot,
                display    = slot.ToString("HH:mm"),
                isoString  = slotUtc.ToString("o"),
                isAvailable = !isBooked
            });
        }

        return Ok(result);
    }

    /// <summary>GET /api/booking/my - Lấy booking của tôi (cần login)</summary>
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
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

    /// <summary>POST /api/booking - Tạo booking mới (cần login)</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(backend.context.booking.api.dtos.CreateBookingRequest request)
    {
        try
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            // Price KHÔNG nhận từ client — handler tự tra DB
            var command = new CreateBookingCommand(
                Guid.Parse(userIdStr),
                request.ServiceId,
                request.BookingTime,
                request.Note
            );

            var bookingId = await _createBookingHandler.HandleAsync(command);
            return Ok(new { Message = "Đặt lịch thành công!", BookingId = bookingId });
        }
        catch (ArgumentException ex)        { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex){ return Conflict(ex.Message); }
        catch (Exception ex)                { return StatusCode(500, "Có lỗi xảy ra: " + ex.Message); }
    }
}
