using System.Collections.Generic;
using System.Security.Claims;
using backend.context.identity.domain.entity;

namespace backend.context.identity.application;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    bool VerifyToken(string token);
}
