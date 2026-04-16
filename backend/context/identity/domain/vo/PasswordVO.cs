using System;

namespace backend.context.identity.domain.vo;

public record PasswordVO
{
    public string Value { get; }

    public PasswordVO(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters long.");
        }

        Value = value;
    }

    public override string ToString() => "********"; // Don't expose password in ToString
}
