using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.naildesign.domain.repo;

namespace backend.context.naildesign.application.queries;

public class GetAllNailDesignsQueryHandler : IQueryHandler<GetAllNailDesignsQuery, IEnumerable<NailDesignResponse>>
{
    private readonly INailDesignRepository _repository;

    public GetAllNailDesignsQueryHandler(INailDesignRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NailDesignResponse>> HandleAsync(GetAllNailDesignsQuery query)
    {
        var designs = await _repository.GetAllActiveAsync();

        return designs.Select(d => new NailDesignResponse(
            d.Id,
            d.Name,
            d.ImageUrl,
            d.OwnerId.Value.ToString(),
            d.Type.ToString(),
            d.Status.ToString(),
            d.Description,
            d.CreatedAt
        ));
    }
}

public class GetNailDesignByIdQueryHandler : IQueryHandler<GetNailDesignByIdQuery, NailDesignResponse?>
{
    private readonly INailDesignRepository _repository;

    public GetNailDesignByIdQueryHandler(INailDesignRepository repository)
    {
        _repository = repository;
    }

    public async Task<NailDesignResponse?> HandleAsync(GetNailDesignByIdQuery query)
    {
        var d = await _repository.GetByIdAsync(query.Id);
        if (d == null) return null;

        return new NailDesignResponse(
            d.Id,
            d.Name,
            d.ImageUrl,
            d.OwnerId.Value.ToString(),
            d.Type.ToString(),
            d.Status.ToString(),
            d.Description,
            d.CreatedAt
        );
    }
}
