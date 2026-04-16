using System;
using backend.context.identity.domain.vo;

namespace backend.context.identity.domain.entity;

public class Auth
{
    public UserIdVO IDNguoidung { get; private set; }
    public string RefreshTokent { get; private set; }
    public string Resettokent { get; private set; }
    public string PasswordHash { get; private set; }

    private Auth(UserIdVO idNguoidung, string passwordHash, string refreshTokent = "", string resettokent = "")
    {
        IDNguoidung = idNguoidung;
        PasswordHash = passwordHash;
        RefreshTokent = refreshTokent;
        Resettokent = resettokent;
    }

    private Auth() 
    {
        IDNguoidung = null!;
        RefreshTokent = null!;
        Resettokent = null!;
        PasswordHash = null!;
    } // Required for EF Core

    // Factory Method
    public static Auth Create(UserIdVO idNguoidung, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash không được để trống.");
        }

        return new Auth(idNguoidung, passwordHash);
    }

    // Cập nhật Token
    public void UpdateRefreshToken(string newToken)
    {
        RefreshTokent = newToken;
    }

    public void UpdateResetToken(string newToken)
    {
        Resettokent = newToken;
    }
    
    public void UpdatePasswordHash(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash))
        {
            throw new ArgumentException("Password hash mới không được để trống.");
        }
        PasswordHash = newHash;
    }
}
