using System;
using backend.context.common.application;

namespace backend.context.identity.application.queries;

public record GetUserQuery(Guid UserId) : IQuery<UserResponse>;

public record UserResponse(
    Guid Id,
    string Ten,
    string Email,
    string SDT,
    string Role
);
