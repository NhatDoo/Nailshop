using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.context.nailservice.domain.repo;

public interface INailServiceRepository
{
    Task<entity.NailService?> GetByIdAsync(Guid id);
    Task<IEnumerable<entity.NailService>> GetAllAsync();
    Task AddAsync(entity.NailService service);
    Task UpdateAsync(entity.NailService service);
    Task DeleteAsync(entity.NailService service);
}
