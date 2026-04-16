using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.api.dtos;
using backend.context.identity.application.commands;
using backend.context.identity.domain.vo;

namespace backend.context.identity.api.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<RegisterUserCommand, UserIdVO> _registerHandler;
    private readonly ICommandHandler<LoginCommand, AuthResponse> _loginHandler;

    public AuthController(
        ICommandHandler<RegisterUserCommand, UserIdVO> registerHandler,
        ICommandHandler<LoginCommand, AuthResponse> loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(
            request.Ten,
            request.SDT,
            request.Email,
            request.Password,
            request.Role
        );

        var userId = await _registerHandler.HandleAsync(command);
        
        return Ok(new { UserId = userId.Value, Message = "Đăng ký thành công!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var response = await _loginHandler.HandleAsync(command);
        
        return Ok(response);
    }
}
