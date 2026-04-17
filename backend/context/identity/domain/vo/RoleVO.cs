using System;

namespace backend.context.identity.domain.vo;

public enum UserRole
{
    Customer,
    Admin
}

public record RoleVO
{
    public UserRole Value { get; }

    public RoleVO(string role)
    {
        // Debug: Enum.TryParse alone is not enough for numeric strings, must use Enum.IsDefined
        if (Enum.TryParse<UserRole>(role, true, out var result) && Enum.IsDefined(typeof(UserRole), result))
        {
            Value = result;
        }
        else
        {
            throw new ArgumentException("Role phải là Customer hoặc Admin.");
        }
    }

    public RoleVO(UserRole value)
    {
        Value = value;
    }

    public static RoleVO Customer => new(UserRole.Customer);
    public static RoleVO Admin => new(UserRole.Admin);

    public override string ToString() => Value.ToString();
}
