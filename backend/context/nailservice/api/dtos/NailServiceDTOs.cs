using System;
using backend.context.nailservice.domain.enums;

namespace backend.context.nailservice.api.dtos;

public record CreateNailServiceRequest(
    string Name,
    string? Description,
    decimal Price,
    int Duration,
    ServiceCategory Category,
    string? Image
);

public record UpdateNailServiceRequest(
    string Name,
    string? Description,
    decimal Price,
    int Duration,
    ServiceCategory Category,
    string? Image
);

public record PromotionDto(
    Guid Id,
    string Title,
    string Description,
    decimal DiscountPercent,
    DateTime StartDate,
    DateTime EndDate
);

public record NailServiceResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Duration,
    string Category,
    string? Image,
    bool IsActive,
    System.Collections.Generic.IEnumerable<PromotionDto> Promotions
);

public record AddPromotionRequest(
    string Title,
    string Description,
    decimal DiscountPercent,
    DateTime StartDate,
    DateTime EndDate
);
