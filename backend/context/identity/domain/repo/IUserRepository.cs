using System.Threading.Tasks;
using backend.context.identity.domain.entity;
using backend.context.identity.domain.vo;

namespace backend.context.identity.domain.repo;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserIdVO id);
    Task<User?> GetByEmailAsync(EmailVO email);
    Task<User?> FindByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
