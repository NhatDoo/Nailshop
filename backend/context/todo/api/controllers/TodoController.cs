using System;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.context.todo.api.dtos;
using backend.context.todo.application.commands;
using backend.context.todo.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.todo.api.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Phải đăng nhập mới được xài Todo
public class TodoController : ControllerBase
{
    private readonly CreateTodoCommandHandler _createHandler;
    private readonly UpdateTodoCommandHandler _updateHandler;
    private readonly ChangeTodoStatusCommandHandler _changeStatusHandler;
    private readonly AddBookingToTodoCommandHandler _addBookingHandler;
    private readonly DeleteTodoCommandHandler _deleteHandler;
    private readonly GetMyTodosQueryHandler _getMyTodosHandler;
    private readonly GetTodoByIdQueryHandler _getByIdHandler;

    public TodoController(
        CreateTodoCommandHandler createHandler,
        UpdateTodoCommandHandler updateHandler,
        ChangeTodoStatusCommandHandler changeStatusHandler,
        AddBookingToTodoCommandHandler addBookingHandler,
        DeleteTodoCommandHandler deleteHandler,
        GetMyTodosQueryHandler getMyTodosHandler,
        GetTodoByIdQueryHandler getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _changeStatusHandler = changeStatusHandler;
        _addBookingHandler = addBookingHandler;
        _deleteHandler = deleteHandler;
        _getMyTodosHandler = getMyTodosHandler;
        _getByIdHandler = getByIdHandler;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    [HttpGet]
    public async Task<IActionResult> GetMyTodos()
    {
        var result = await _getMyTodosHandler.HandleAsync(new GetMyTodosQuery(CurrentUserId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTodoById(Guid id)
    {
        var result = await _getByIdHandler.HandleAsync(new GetTodoByIdQuery(id, CurrentUserId));
        if (result == null) return NotFound("Todo không tồn tại hoặc bạn không có quyền xem.");
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoRequest request)
    {
        try
        {
            var command = new CreateTodoCommand(
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate,
                CurrentUserId
            );

            var id = await _createHandler.HandleAsync(command);
            return CreatedAtAction(nameof(GetTodoById), new { id }, new { Message = "Tạo Todo thành công", Id = id });
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTodoRequest request)
    {
        try
        {
            var command = new UpdateTodoCommand(
                id,
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate,
                CurrentUserId
            );
            await _updateHandler.HandleAsync(command);
            return Ok(new { Message = "Cập nhật thành công!" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeTodoStatusRequest request)
    {
        try
        {
            var command = new ChangeTodoStatusCommand(id, request.Status, CurrentUserId);
            await _changeStatusHandler.HandleAsync(command);
            return Ok(new { Message = "Đổi trạng thái thành công!" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("{id:guid}/bookings")]
    public async Task<IActionResult> AddBooking(Guid id, AddTodoBookingRequest request)
    {
        try
        {
            var command = new AddBookingToTodoCommand(id, request.BookingId, CurrentUserId);
            await _addBookingHandler.HandleAsync(command);
            return Ok(new { Message = "Thêm Booking vào Todo thành công!" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _deleteHandler.HandleAsync(new DeleteTodoCommand(id, CurrentUserId));
            return Ok(new { Message = "Xóa Todo thành công!" });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
