using System;
using backend.context.common.application;
using backend.context.nailservice.domain.enums;
using backend.context.nailservice.domain.repo;
using System.Threading.Tasks;

namespace backend.context.nailservice.application.commands;

public record UpdateNailServiceCommand(
    Guid Id, string Name, string? Description, decimal Price, 
    int Duration, ServiceCategory Category, string? Image) : ICommand<bool>;

public class UpdateNailServiceCommandHandler : ICommandHandler<UpdateNailServiceCommand, bool>
{
    private readonly INailServiceRepository _repository;

    public UpdateNailServiceCommandHandler(INailServiceRepository repository)
        => _repository = repository;

    public async Task<bool> HandleAsync(UpdateNailServiceCommand command)
    {
        var service = await _repository.GetByIdAsync(command.Id);
        if (service == null) throw new ArgumentException("Service not found");

        service.Update(command.Name, command.Description, command.Price, 
            command.Duration, command.Category, command.Image);

        await _repository.UpdateAsync(service);
        return true;
    }
}
