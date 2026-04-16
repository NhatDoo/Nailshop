using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using backend.context.identity.application;
using backend.context.identity.domain.entity;
// Lưu ý: Cần cài đặt gói Microsoft.IdentityModel.Tokens và System.IdentityModel.Tokens.Jwt
// Do luật không cho phép mình tự edit file config/csproj nên bạn hãy chạy lệnh sau:
// dotnet add package System.IdentityModel.Tokens.Jwt

using Microsoft.Extensions.Configuration;

namespace backend.context.identity.infrastructure;

public class TokenService : ITokenService
{
    private readonly string _secretKey;

    public TokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? "FallbackSecretKeyForDevelopmentOnly";
    }
    public string GenerateAccessToken(User user)
    {
        // Đây là bản mô phỏng logic sinh token (Vì chưa có thư viện JWT trong project)
        // Khi bạn đã cài thư viện, mình sẽ cập nhật code JWT chuẩn
        var token = $"access_token_for_{user.Email.Value}_{Guid.NewGuid()}";
        return token;
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        // Logic verify và lấy claims từ token hết hạn
        return null;
    }

    public bool VerifyToken(string token)
    {
        // Logic verify token
        return !string.IsNullOrEmpty(token);
    }
}
