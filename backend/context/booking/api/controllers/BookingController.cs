using System;
using System.Threading.Tasks;
using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.booking.api.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Phải đăng nhập mới được đặt lịch
public class BookingController : ControllerBase
{
    private readonly CreateBookingCommandHandler _createBookingHandler;
    private readonly GetMyBookingsQueryHandler _getMyBookingsHandler;

    public BookingController(
        CreateBookingCommandHandler createBookingHandler,
        GetMyBookingsQueryHandler getMyBookingsHandler)
    {
        _createBookingHandler = createBookingHandler;
        _getMyBookingsHandler = getMyBookingsHandler;
    }

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
