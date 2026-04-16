using System;
using backend.context.identity.domain.vo;

namespace backend.context.identity.application.queries;

public record GetUserQuery(Guid UserId);

public record UserResponse(
    Guid Id,
    string Ten,
    string Email,
    string SDT,
    string Role
);
