using backend.context.common.application;

namespace backend.context.naildesign.application.commands;

public record CreateNailDesignCommand(
    string Name,
    string ImageUrl,
    string OwnerId,   // UserId của Admin đang thực hiện
    string Type,      // "Preset" | "Custom"
    string? Description
) : ICommand<System.Guid>;
