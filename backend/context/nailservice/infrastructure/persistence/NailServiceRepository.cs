using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.context.nailservice.domain.repo;

namespace backend.context.nailservice.infrastructure.persistence;

public class NailServiceRepository : INailServiceRepository
{
    private readonly AppDbContext _context;

    public NailServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<domain.entity.NailService?> GetByIdAsync(Guid id)
    {
        return await _context.Set<domain.entity.NailService>()
            .FirstOrDefaultAsync(s => s.Id == id); // OwnsMany tự eager load
    }

    public async Task<IEnumerable<domain.entity.NailService>> GetAllAsync()
    {
        return await _context.Set<domain.entity.NailService>().ToListAsync();
    }

    public async Task AddAsync(domain.entity.NailService service)
    {
        await _context.Set<domain.entity.NailService>().AddAsync(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(domain.entity.NailService service)
    {
        _context.Set<domain.entity.NailService>().Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(domain.entity.NailService service)
    {
        _context.Set<domain.entity.NailService>().Remove(service);
        await _context.SaveChangesAsync();
    }
}
