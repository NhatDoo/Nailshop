using System;
using backend.context.common.application;
using backend.context.nailservice.domain.repo;
using System.Threading.Tasks;

namespace backend.context.nailservice.application.commands;

public record ToggleNailServiceStatusCommand(Guid Id) : ICommand<bool>;

public class ToggleNailServiceStatusCommandHandler : ICommandHandler<ToggleNailServiceStatusCommand, bool>
{
    private readonly INailServiceRepository _repository;

    public ToggleNailServiceStatusCommandHandler(INailServiceRepository repository)
        => _repository = repository;

    public async Task<bool> HandleAsync(ToggleNailServiceStatusCommand command)
    {
        var service = await _repository.GetByIdAsync(command.Id);
        if (service == null) throw new ArgumentException("Service not found");

        service.ToggleStatus();
        await _repository.UpdateAsync(service);
        return true;
    }
}

// Add/Remove Promotion Commands
public record AddPromotionCommand(Guid NailServiceId, string Title, string Description, decimal DiscountPercent, DateTime StartDate, DateTime EndDate) : ICommand<bool>;

public class AddPromotionCommandHandler : ICommandHandler<AddPromotionCommand, bool>
{
    private readonly INailServiceRepository _repository;
    public AddPromotionCommandHandler(INailServiceRepository repository) => _repository = repository;

    public async Task<bool> HandleAsync(AddPromotionCommand command)
    {
        var service = await _repository.GetByIdAsync(command.NailServiceId);
        if (service == null) throw new ArgumentException("Service not found");

        service.AddPromotion(command.Title, command.Description, command.DiscountPercent, command.StartDate, command.EndDate);
        await _repository.UpdateAsync(service);
        return true;
    }
}
