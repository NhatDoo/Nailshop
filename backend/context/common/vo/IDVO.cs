using System;

namespace backend.context.common.vo;

public abstract record IDVO
{
    public Guid Value { get; }

    protected IDVO(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.");
        }
        Value = value;
    }

    public override string ToString() => Value.ToString();
}
