using System;

namespace backend.context.common.domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
