using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.naildesign.api.dtos;
using backend.context.naildesign.application.commands;
using backend.context.naildesign.application.queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.context.naildesign.api.controllers;

[ApiController]
[Route("api/[controller]")]
public class NailDesignController : ControllerBase
{
    private const string BucketName = "nail-designs";
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    private readonly CreateNailDesignCommandHandler _createHandler;
    private readonly UpdateNailDesignCommandHandler _updateHandler;
    private readonly DeleteNailDesignCommandHandler _deleteHandler;
    private readonly GetAllNailDesignsQueryHandler _getAllHandler;
    private readonly GetNailDesignByIdQueryHandler _getByIdHandler;
    private readonly IStorageService _storageService;

    public NailDesignController(
        CreateNailDesignCommandHandler createHandler,
        UpdateNailDesignCommandHandler updateHandler,
        DeleteNailDesignCommandHandler deleteHandler,
        GetAllNailDesignsQueryHandler getAllHandler,
        GetNailDesignByIdQueryHandler getByIdHandler,
        IStorageService storageService)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _storageService = storageService;
    }

    /// <summary>GET /api/naildesign - Lấy tất cả mẫu nail Active (public)</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getAllHandler.HandleAsync(new GetAllNailDesignsQuery());
        return Ok(result);
    }

    /// <summary>GET /api/naildesign/{id} (public)</summary>
    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getByIdHandler.HandleAsync(new GetNailDesignByIdQuery(id));
        return result == null ? NotFound($"Không tìm thấy mẫu nail với Id: {id}") : Ok(result);
    }

    /// <summary>POST /api/naildesign - Tạo mẫu nail + upload ảnh lên MinIO (Admin)</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateNailDesignRequest request)
    {
        try
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(ownerId)) return Unauthorized();

            // --- Validate file ---
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("Vui lòng chọn file ảnh.");

            if (request.Image.Length > MaxFileSizeBytes)
                return BadRequest("Ảnh không được vượt quá 5MB.");

            var ext = Path.GetExtension(request.Image.FileName).ToLowerInvariant();
            if (!Array.Exists(AllowedExtensions, e => e == ext))
                return BadRequest($"Chỉ chấp nhận định dạng: {string.Join(", ", AllowedExtensions)}.");

            // --- Upload lên MinIO ---
            var fileName = $"{Guid.NewGuid()}{ext}"; // Tên file ngẫu nhiên để tránh trùng
            using var stream = request.Image.OpenReadStream();
            var imageUrl = await _storageService.UploadAsync(
                stream, fileName, request.Image.ContentType, BucketName);

            // --- Tạo NailDesign Entity ---
            var command = new CreateNailDesignCommand(
                request.Name,
                imageUrl,
                ownerId,
                request.Type,
                request.Description
            );
            var id = await _createHandler.HandleAsync(command);

            return CreatedAtAction(nameof(GetById), new { id },
                new { Message = "Tạo mẫu nail thành công!", Id = id, ImageUrl = imageUrl });
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return StatusCode(500, $"Lỗi hệ thống: {ex.Message}"); }
    }

    /// <summary>PUT /api/naildesign/{id} (Admin)</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateNailDesignRequest request)
    {
        try
        {
            var command = new UpdateNailDesignCommand(id, request.Name, request.ImageUrl, request.Description);
            await _updateHandler.HandleAsync(command);
            return Ok(new { Message = "Cập nhật thành công!" });
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { return NotFound(ex.Message); }
    }

    /// <summary>DELETE /api/naildesign/{id} - Soft Delete (Admin)</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _deleteHandler.HandleAsync(new DeleteNailDesignCommand(id));
            return Ok(new { Message = "Xóa mẫu nail thành công!" });
        }
        catch (Exception ex) { return NotFound(ex.Message); }
    }
}
