using backend.context.naildesign.application.commands;
using backend.context.naildesign.application.queries;
using backend.context.naildesign.domain.repo;
using backend.context.naildesign.infrastructure.persistence;
using Microsoft.Extensions.DependencyInjection;

namespace backend.context.naildesign;

public static class NailDesignModule
{
    public static IServiceCollection AddNailDesignModule(this IServiceCollection services)
    {
        // Repository
        services.AddScoped<INailDesignRepository, NailDesignRepository>();

        // Command Handlers
        services.AddScoped<CreateNailDesignCommandHandler>();
        services.AddScoped<UpdateNailDesignCommandHandler>();
        services.AddScoped<DeleteNailDesignCommandHandler>();

        // Query Handlers
        services.AddScoped<GetAllNailDesignsQueryHandler>();
        services.AddScoped<GetNailDesignByIdQueryHandler>();

        return services;
    }
}
