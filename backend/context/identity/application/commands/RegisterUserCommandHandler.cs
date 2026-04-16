using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.domain.entity;
using backend.context.identity.domain.repo;
using backend.context.identity.domain.vo;

namespace backend.context.identity.application.commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserIdVO>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserIdVO> HandleAsync(RegisterUserCommand command)
    {
        // 1. Kiểm tra Email tồn tại
        var existingUser = await _userRepository.GetByEmailAsync(new EmailVO(command.Email));
        if (existingUser != null)
        {
            throw new Exception("Email đã tồn tại trong hệ thống.");
        }

        // 2. Tạo thực thể User (Aggregate Root tự tạo Auth bên trong)
        var passwordHash = _passwordHasher.HashPassword(command.Password);
        var user = User.Create(command.Ten, command.SDT, command.Email, command.Role, passwordHash);

        // 3. Lưu vào Repository (EF sẽ tự động lưu cả Auth đính kèm)
        await _userRepository.AddAsync(user);

        // 4. Trả về ID người dùng mới
        return user.Id;
    }
}
