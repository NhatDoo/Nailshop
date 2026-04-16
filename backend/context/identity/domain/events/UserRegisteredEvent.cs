using System;
using backend.context.identity.domain.vo;

namespace backend.context.identity.domain.events;

public record UserRegisteredEvent
{
    public UserIdVO UserId { get; }
    public EmailVO Email { get; }
    public DateTime OccurredOn { get; }

    public UserRegisteredEvent(UserIdVO userId, EmailVO email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
}
