using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.domain.vo;
using backend.context.naildesign.domain.entity;
using backend.context.naildesign.domain.repo;

namespace backend.context.naildesign.application.commands;

public class CreateNailDesignCommandHandler : ICommandHandler<CreateNailDesignCommand, Guid>
{
    private readonly INailDesignRepository _repository;

    public CreateNailDesignCommandHandler(INailDesignRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> HandleAsync(CreateNailDesignCommand command)
    {
        var nailDesign = NailDesign.Create(
            command.Name,
            command.ImageUrl,
            new UserIdVO(Guid.Parse(command.OwnerId)),
            command.Type,
            command.Description
        );

        await _repository.AddAsync(nailDesign);
        return nailDesign.Id;
    }
}
