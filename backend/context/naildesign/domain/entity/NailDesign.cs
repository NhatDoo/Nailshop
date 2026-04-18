using System;
using backend.context.common.domain;
using backend.context.identity.domain.vo;
using backend.context.naildesign.domain.vo;

namespace backend.context.naildesign.domain.entity;

public class NailDesign : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; private set; }
    public UserIdVO OwnerId { get; private set; }  // Liên kết với Identity module
    public NailDesignType Type { get; private set; }
    public NailDesignStatus Status { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private NailDesign() { } // Required for EF Core

    private NailDesign(
        Guid id,
        string name,
        string imageUrl,
        UserIdVO ownerId,
        NailDesignType type,
        string? description)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
        OwnerId = ownerId;
        Type = type;
        Description = description;
        Status = NailDesignStatus.Active; // Mặc định là Active khi tạo mới
        CreatedAt = DateTime.UtcNow;
    }

    // Factory Method - Invariant: Chỉ Admin mới được gọi phương thức này (kiểm tra ở tầng Application)
    public static NailDesign Create(
        string name,
        string imageUrl,
        UserIdVO ownerId,
        string type,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên mẫu nail không được để trống.");

        if (name.Length > 100)
            throw new ArgumentException("Tên mẫu nail không được dài quá 100 ký tự.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("URL hình ảnh không được để trống.");

        // Chặn chuỗi số ("0", "1") và giá trị không hợp lệ
        if (!Enum.TryParse<NailDesignType>(type, ignoreCase: true, out var designType)
            || !Enum.IsDefined(typeof(NailDesignType), designType)
            || int.TryParse(type, out _))
            throw new ArgumentException($"Loại mẫu nail không hợp lệ. Các giá trị hợp lệ: Preset, Custom.");

        return new NailDesign(Guid.NewGuid(), name, imageUrl, ownerId, designType, description);
    }

    // Business Methods
    public void Update(string name, string imageUrl, string? description)
    {
        if (Status == NailDesignStatus.Deleted)
            throw new InvalidOperationException("Không thể cập nhật mẫu nail đã bị xóa.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên mẫu nail không được để trống.");

        if (name.Length > 100)
            throw new ArgumentException("Tên mẫu nail không được dài quá 100 ký tự.");

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("URL hình ảnh không được để trống.");

        Name = name;
        ImageUrl = imageUrl;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    // Soft Delete - Không xóa cứng khỏi DB
    public void Delete()
    {
        if (Status == NailDesignStatus.Deleted)
            throw new InvalidOperationException("Mẫu nail này đã bị xóa trước đó.");

        Status = NailDesignStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }
}
