namespace backend.context.identity.api.dtos;

public record RegisterRequest(
    string Ten,
    string SDT,
    string Email,
    string Password,
    string Role
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string Email,
    string Role
);

public record ForgotPasswordRequest(string Email);
