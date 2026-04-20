using backend.context.common.application;
using backend.context.identity.domain.repo;
using Microsoft.Extensions.Configuration;
using backend.context.notification.application.services;

namespace backend.context.identity.application.commands;

public record ForgotPasswordCommand(string Email) : ICommand<bool>;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<bool> HandleAsync(ForgotPasswordCommand command)
    {
        var user = await _userRepository.FindByEmailAsync(command.Email);
        if (user == null)
            // Không tiết lộ email có tồn tại hay không (bảo mật)
            return true;

        // Tạo token reset ngẫu nhiên (giản lược, trong thực tế nên lưu DB với expiry)
        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "-").Replace("/", "_").TrimEnd('=');

        var resetLink = $"http://localhost:5173/reset-password?token={resetToken}&email={Uri.EscapeDataString(command.Email)}";

        var htmlBody = $"""
            <div style="font-family:Arial,sans-serif;max-width:500px;margin:auto;padding:20px;border:1px solid #f0c9d8;border-radius:8px;">
                <h2 style="color:#e91e8c;">🌸 Nailshop - Đặt Lại Mật Khẩu</h2>
                <p>Xin chào <strong>{user.Ten}</strong>,</p>
                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                <p>Nhấn vào nút bên dưới để đặt lại mật khẩu (hiệu lực trong 15 phút):</p>
                <a href="{resetLink}" style="display:inline-block;padding:12px 24px;background:#e91e8c;color:white;text-decoration:none;border-radius:5px;margin:16px 0;">
                    Đặt Lại Mật Khẩu
                </a>
                <p style="color:#999;font-size:13px;">Nếu bạn không yêu cầu điều này, hãy bỏ qua email này.</p>
            </div>
        """;

        await _emailService.SendEmailAsync(command.Email, "Đặt lại mật khẩu - Nailshop", htmlBody);
        return true;
    }
}
