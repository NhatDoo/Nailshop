using System;
using System.Collections.Generic;
using backend.context.common.application;

namespace backend.context.naildesign.application.queries;

// DTOs trả về cho Client
public record NailDesignResponse(
    Guid Id,
    string Name,
    string ImageUrl,
    string OwnerId,
    string Type,
    string Status,
    string? Description,
    DateTime CreatedAt
);

// Lấy tất cả mẫu nail đang Active
public record GetAllNailDesignsQuery() : IQuery<IEnumerable<NailDesignResponse>>;

// Lấy một mẫu nail theo Id
public record GetNailDesignByIdQuery(Guid Id) : IQuery<NailDesignResponse?>;
