using System.Threading;
using System.Threading.Tasks;
using Domain.Contexts;
using Domain.Models.Rates;
using MediatR;

namespace DomainServices.Queries.Currencies.Rates.GetAllRatesForDate;

public class GetAllRatesForDateHandler : IRequestHandler<GetAllRatesForDateQuery, RateForDateList>
{
    private readonly IContext _context;

    public GetAllRatesForDateHandler(IContext context) => _context = context;

    public async Task<RateForDateList> Handle(GetAllRatesForDateQuery request, CancellationToken cancellationToken) =>
        await _context
            .GetAllRatesForDate(request.Specification.DateTime)
            .ConfigureAwait(false);
}
