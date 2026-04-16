using backend.context.identity.api.dtos;
using backend.context.common.application;

namespace backend.context.identity.application.commands;

public record LoginCommand(string Email, string Password);
