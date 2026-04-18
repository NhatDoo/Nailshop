using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.naildesign.domain.repo;

namespace backend.context.naildesign.application.commands;

public class UpdateNailDesignCommandHandler : ICommandHandler<UpdateNailDesignCommand, bool>
{
    private readonly INailDesignRepository _repository;

    public UpdateNailDesignCommandHandler(INailDesignRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(UpdateNailDesignCommand command)
    {
        var nailDesign = await _repository.GetByIdAsync(command.Id)
            ?? throw new Exception($"Không tìm thấy mẫu nail với Id: {command.Id}");

        nailDesign.Update(command.Name, command.ImageUrl, command.Description);
        await _repository.UpdateAsync(nailDesign);
        return true;
    }
}

public class DeleteNailDesignCommandHandler : ICommandHandler<DeleteNailDesignCommand, bool>
{
    private readonly INailDesignRepository _repository;

    public DeleteNailDesignCommandHandler(INailDesignRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteNailDesignCommand command)
    {
        var nailDesign = await _repository.GetByIdAsync(command.Id)
            ?? throw new Exception($"Không tìm thấy mẫu nail với Id: {command.Id}");

        nailDesign.Delete(); // Soft Delete - chỉ đổi Status sang Deleted
        await _repository.UpdateAsync(nailDesign);
        return true;
    }
}
