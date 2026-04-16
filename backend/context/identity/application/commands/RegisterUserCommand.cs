namespace backend.context.identity.application.commands;

public record RegisterUserCommand(
    string Ten,
    string SDT,
    string Email,
    string Password,
    string Role
);
