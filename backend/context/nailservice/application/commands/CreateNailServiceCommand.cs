using System;
using backend.context.common.application;
using backend.context.nailservice.domain.enums;
using backend.context.nailservice.domain.repo;
using System.Threading.Tasks;

namespace backend.context.nailservice.application.commands;

public record CreateNailServiceCommand(
    string Name, string? Description, decimal Price, int Duration, 
    ServiceCategory Category, string? Image) : ICommand<Guid>;

public class CreateNailServiceCommandHandler : ICommandHandler<CreateNailServiceCommand, Guid>
{
    private readonly INailServiceRepository _repository;

    public CreateNailServiceCommandHandler(INailServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> HandleAsync(CreateNailServiceCommand command)
    {
        var service = domain.entity.NailService.Create(
            command.Name, command.Description, command.Price, 
            command.Duration, command.Category, command.Image);
        
        await _repository.AddAsync(service);
        return service.Id;
    }
}
