using backend.context.booking.application.commands;
using backend.context.booking.application.queries;
using backend.context.booking.domain.repo;
using backend.context.booking.infrastructure.persistence;
using Microsoft.Extensions.DependencyInjection;

namespace backend.context.booking;

public static class BookingModule
{
    public static IServiceCollection AddBookingModule(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Handlers
        services.AddScoped<CreateBookingCommandHandler>();
        services.AddScoped<GetMyBookingsQueryHandler>();
        services.AddScoped<GetAllBookingsForDateQueryHandler>();
        services.AddScoped<backend.context.booking.application.events.BookingCreatedEventHandler>();

        return services;
    }
}
