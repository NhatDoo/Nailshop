using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.context.naildesign.domain.entity;

namespace backend.context.naildesign.domain.repo;

public interface INailDesignRepository
{
    Task AddAsync(NailDesign nailDesign);
    Task<NailDesign?> GetByIdAsync(Guid id);
    Task<IEnumerable<NailDesign>> GetAllActiveAsync();
    Task UpdateAsync(NailDesign nailDesign);
}
