using System.Threading;
using System.Threading.Tasks;
using Domain.Contexts;
using MediatR;

namespace DomainServices.Commands.Rates.InsertRateForDateList;

public class InsertRateForDateListHandler : IRequestHandler<InsertRateForDateListCommand, int>
{
    private readonly IContext _context;

    public InsertRateForDateListHandler(IContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(InsertRateForDateListCommand request, CancellationToken cancellationToken) =>
        await _context
            .InsertRateForDateList(request.Specification.List)
            .ConfigureAwait(false);
}
