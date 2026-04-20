using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using backend.context.identity.application;
using backend.context.identity.domain.repo;
using backend.context.identity.infrastructure;
using backend.context.identity.infrastructure.persistence;
using backend.context.identity.application.commands;
using backend.context.identity.application.queries;
using backend.context.identity.application.events;
using backend.context.common.application;
using backend.context.identity.domain.vo;
using backend.context.identity.api.dtos;

namespace backend.context.identity;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        // 1. Infrastructure
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // 2. Application Handlers (Commands/Queries)
        services.AddScoped<ICommandHandler<RegisterUserCommand, UserIdVO>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<ForgotPasswordCommand, bool>, ForgotPasswordCommandHandler>();
        services.AddScoped<IQueryHandler<GetUserQuery, UserResponse>, GetUserQueryHandler>();
        
        // 3. Events
        services.AddScoped<UserRegisteredEventHandler>();

        return services;
    }
}
