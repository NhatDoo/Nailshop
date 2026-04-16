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
        if (Enum.TryParse<UserRole>(role, true, out var result))
        {
            Value = result;
        }
        else
        {
            throw new ArgumentException("Role must be either 'Customer' or 'Admin'.");
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
