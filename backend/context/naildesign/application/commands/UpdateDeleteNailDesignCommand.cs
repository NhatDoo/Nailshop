using System;
using backend.context.common.application;

namespace backend.context.naildesign.application.commands;

public record UpdateNailDesignCommand(
    Guid Id,
    string Name,
    string ImageUrl,
    string? Description
) : ICommand<bool>;

public record DeleteNailDesignCommand(Guid Id) : ICommand<bool>;
