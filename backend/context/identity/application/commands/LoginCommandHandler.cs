using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.api.dtos;
using backend.context.identity.domain.repo;
using backend.context.identity.domain.vo;
using backend.context.identity.application;

namespace backend.context.identity.application.commands;

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> HandleAsync(LoginCommand command)
    {
        // 1. Tìm User kèm thông tin Auth (Aggregate)
        var user = await _userRepository.GetByEmailAsync(new EmailVO(command.Email));
        
        // 2. Kiểm tra sự tồn tại và Validate Password
        if (user == null || !_passwordHasher.VerifyPassword(command.Password, user.Auth.PasswordHash))
        {
            throw new Exception("Email hoặc mật khẩu không chính xác.");
        }

        // 3. Sinh Token
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // 4. Cập nhật Refresh Token trực tiếp qua Aggregate Root
        user.Auth.UpdateRefreshToken(refreshToken);
        await _userRepository.UpdateAsync(user);

        return new AuthResponse(
            accessToken,
            refreshToken,
            user.Email.Value,
            user.Role.Value.ToString()
        );
    }
}
