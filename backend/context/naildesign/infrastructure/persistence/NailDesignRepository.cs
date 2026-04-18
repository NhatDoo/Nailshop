using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.naildesign.domain.entity;
using backend.context.naildesign.domain.repo;
using backend.context.naildesign.domain.vo;
using Microsoft.EntityFrameworkCore;

namespace backend.context.naildesign.infrastructure.persistence;

public class NailDesignRepository : INailDesignRepository
{
    private readonly AppDbContext _context;

    public NailDesignRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NailDesign nailDesign)
    {
        await _context.NailDesigns.AddAsync(nailDesign);
        await _context.SaveChangesAsync();
    }

    public async Task<NailDesign?> GetByIdAsync(Guid id)
    {
        return await _context.NailDesigns.FindAsync(id);
    }

    public async Task<IEnumerable<NailDesign>> GetAllActiveAsync()
    {
        return await _context.NailDesigns
            .Where(n => n.Status == NailDesignStatus.Active)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(NailDesign nailDesign)
    {
        _context.NailDesigns.Update(nailDesign);
        await _context.SaveChangesAsync();
    }
}
