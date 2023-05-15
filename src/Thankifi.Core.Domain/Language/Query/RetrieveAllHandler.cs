using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Core.Domain.Contract.Language.Queries;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Language.Query;

public class RetrieveAllHandler : QueryHandler<RetrieveAll, PaginatedList<LanguageDto>>
{
    private readonly ThankifiDbContext _dbContext;

    public RetrieveAllHandler(ThankifiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<PaginatedList<LanguageDto>> Handle(RetrieveAll request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Languages.AsNoTracking();
            
        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(language => new LanguageDto
            {
                Id = language.Id,
                Code = language.Code
            }).ToListAsync(cancellationToken);
            
        return new PaginatedList<LanguageDto>(items, count, request.PageNumber, request.PageSize);
    }
}