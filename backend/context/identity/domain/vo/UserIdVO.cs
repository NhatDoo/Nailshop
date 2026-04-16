using System;
using backend.context.common.vo;

namespace backend.context.identity.domain.vo;

public record UserIdVO : IDVO
{
    public UserIdVO(Guid value) : base(value)
    {
    }

    public static UserIdVO Create() => new(Guid.NewGuid());
}
