using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.context.identity.domain.entity;
using backend.context.identity.domain.repo;
using backend.context.identity.domain.vo;

namespace backend.context.identity.infrastructure.persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(UserIdVO id)
    {
        return await _context.Set<User>()
            .Include(u => u.Auth)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(EmailVO email)
    {
        return await _context.Set<User>()
            .Include(u => u.Auth)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Set<User>()
            .Include(u => u.Auth)
            .FirstOrDefaultAsync(u => u.Email == new EmailVO(email));
    }

    public async Task AddAsync(User user)
    {
        await _context.Set<User>().AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Set<User>().Update(user);
        await _context.SaveChangesAsync();
    }
}
