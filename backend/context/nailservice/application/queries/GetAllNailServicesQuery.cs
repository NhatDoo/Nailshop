using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.context.common.application;
using backend.context.nailservice.api.dtos;
using backend.context.nailservice.domain.repo;

namespace backend.context.nailservice.application.queries;

public record GetAllNailServicesQuery() : IQuery<IEnumerable<NailServiceResponse>>;

public class GetAllNailServicesQueryHandler : IQueryHandler<GetAllNailServicesQuery, IEnumerable<NailServiceResponse>>
{
    private readonly INailServiceRepository _repository;

    public GetAllNailServicesQueryHandler(INailServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<NailServiceResponse>> HandleAsync(GetAllNailServicesQuery query)
    {
        var services = await _repository.GetAllAsync();
        return services.Select(s => new NailServiceResponse(
            s.Id,
            s.Name,
            s.Description,
            s.Price,
            s.Duration,
            s.Category.ToString(),
            s.Image,
            s.IsActive,
            s.Promotions.Select(p => new PromotionDto(
                p.Id, p.Title, p.Description, p.DiscountPercent, p.StartDate, p.EndDate
            ))
        ));
    }
}
