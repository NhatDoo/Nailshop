using System;

namespace backend.context.identity.domain.vo;

public record PasswordVO
{
    public string Value { get; }

    public PasswordVO(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 8 || value.Length > 100)
        {
            throw new ArgumentException("Mật khẩu phải từ 8 đến 100 ký tự.");
        }

        Value = value;
    }

    public override string ToString() => "********"; // Don't expose password in ToString
}
