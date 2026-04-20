using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.nailservice.api.dtos;
using backend.context.nailservice.application.commands;
using backend.context.nailservice.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.nailservice.api.controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class NailServiceController : ControllerBase
{
    private readonly ICommandHandler<CreateNailServiceCommand, Guid> _createHandler;
    private readonly ICommandHandler<UpdateNailServiceCommand, bool> _updateHandler;
    private readonly ICommandHandler<ToggleNailServiceStatusCommand, bool> _toggleHandler;
    private readonly ICommandHandler<AddPromotionCommand, bool> _addPromoHandler;
    private readonly IQueryHandler<GetAllNailServicesQuery, System.Collections.Generic.IEnumerable<NailServiceResponse>> _getAllHandler;

    public NailServiceController(
        ICommandHandler<CreateNailServiceCommand, Guid> createHandler,
        ICommandHandler<UpdateNailServiceCommand, bool> updateHandler,
        ICommandHandler<ToggleNailServiceStatusCommand, bool> toggleHandler,
        ICommandHandler<AddPromotionCommand, bool> addPromoHandler,
        IQueryHandler<GetAllNailServicesQuery, System.Collections.Generic.IEnumerable<NailServiceResponse>> getAllHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _toggleHandler = toggleHandler;
        _addPromoHandler = addPromoHandler;
        _getAllHandler = getAllHandler;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var res = await _getAllHandler.HandleAsync(new GetAllNailServicesQuery());
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNailServiceRequest req)
    {
        try
        {
            var id = await _createHandler.HandleAsync(new CreateNailServiceCommand(
                req.Name, req.Description, req.Price, req.Duration, req.Category, req.Image));
            return Ok(new { Message = "Tạo dịch vụ thành công", Id = id });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNailServiceRequest req)
    {
        try
        {
            await _updateHandler.HandleAsync(new UpdateNailServiceCommand(
                id, req.Name, req.Description, req.Price, req.Duration, req.Category, req.Image));
            return Ok(new { Message = "Cập nhật thành công" });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            await _toggleHandler.HandleAsync(new ToggleNailServiceStatusCommand(id));
            return Ok(new { Message = "Đổi trạng thái thành công" });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("{id:guid}/promotions")]
    public async Task<IActionResult> AddPromotion(Guid id, [FromBody] AddPromotionRequest req)
    {
        try
        {
            await _addPromoHandler.HandleAsync(new AddPromotionCommand(
                id, req.Title, req.Description, req.DiscountPercent, req.StartDate, req.EndDate));
            return Ok(new { Message = "Thêm ưu đãi thành công" });
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }
}
