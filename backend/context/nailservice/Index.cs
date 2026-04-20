using Microsoft.Extensions.DependencyInjection;
using backend.context.nailservice.domain.repo;
using backend.context.nailservice.infrastructure.persistence;
using backend.context.nailservice.application.commands;
using backend.context.nailservice.application.queries;
using backend.context.common.application;
using System.Collections.Generic;
using System;
using backend.context.nailservice.api.dtos;

namespace backend.context.nailservice;

public static class Index
{
    public static IServiceCollection AddNailServiceModule(this IServiceCollection services)
    {
        services.AddScoped<INailServiceRepository, NailServiceRepository>();
        
        services.AddScoped<ICommandHandler<CreateNailServiceCommand, Guid>, CreateNailServiceCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateNailServiceCommand, bool>, UpdateNailServiceCommandHandler>();
        services.AddScoped<ICommandHandler<ToggleNailServiceStatusCommand, bool>, ToggleNailServiceStatusCommandHandler>();
        services.AddScoped<ICommandHandler<AddPromotionCommand, bool>, AddPromotionCommandHandler>();

        services.AddScoped<IQueryHandler<GetAllNailServicesQuery, IEnumerable<NailServiceResponse>>, GetAllNailServicesQueryHandler>();

        return services;
    }
}
