using Microsoft.AspNetCore.Http;

namespace backend.context.naildesign.api.dtos;

// Nhận qua multipart/form-data
public class CreateNailDesignRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;    // "Preset" | "Custom"
    public string? Description { get; set; }
    public IFormFile Image { get; set; } = null!;       // File ảnh upload lên
}

public record UpdateNailDesignRequest(
    string Name,
    string ImageUrl,
    string? Description
);
