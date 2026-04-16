using System;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.identity.domain.repo;
using backend.context.identity.domain.vo;

namespace backend.context.identity.application.queries;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> HandleAsync(GetUserQuery query)
    {
        var user = await _userRepository.GetByIdAsync(new UserIdVO(query.UserId));
        
        if (user == null)
        {
            throw new Exception("Không tìm thấy người dùng.");
        }

        return new UserResponse(
            user.Id.Value,
            user.Ten,
            user.Email.Value,
            user.SDT.Value,
            user.Role.Value.ToString()
        );
    }
}
