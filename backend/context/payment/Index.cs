using backend.context.payment.application.commands;
using backend.context.payment.application.services;
using backend.context.payment.domain.repo;
using backend.context.payment.infrastructure.persistence;
using backend.context.payment.infrastructure.services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.context.payment;

public static class PaymentModule
{
    public static IServiceCollection AddPaymentModule(this IServiceCollection services)
    {
        // Repository
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Services
        services.AddScoped<IVnpayService, VnpayService>();

        // Command Handlers
        services.AddScoped<CreateVnpayPaymentCommandHandler>();
        services.AddScoped<ProcessVnpayCallbackCommandHandler>();

        return services;
    }
}
