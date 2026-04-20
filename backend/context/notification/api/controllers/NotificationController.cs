using System;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.context.notification.application.commands;
using backend.context.notification.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.notification.api.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly CreateNotificationCommandHandler _createHandler;
    private readonly MarkNotificationAsReadCommandHandler _markReadHandler;
    private readonly GetMyNotificationsQueryHandler _getMyNotificationsHandler;

    public NotificationController(
        CreateNotificationCommandHandler createHandler,
        MarkNotificationAsReadCommandHandler markReadHandler,
        GetMyNotificationsQueryHandler getMyNotificationsHandler)
    {
        _createHandler = createHandler;
        _markReadHandler = markReadHandler;
        _getMyNotificationsHandler = getMyNotificationsHandler;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var result = await _getMyNotificationsHandler.HandleAsync(new GetMyNotificationsQuery(CurrentUserId));
        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            await _markReadHandler.HandleAsync(new MarkNotificationAsReadCommand(id, CurrentUserId));
            return Ok(new { Message = "Đánh dấu đã đọc thành công." });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
