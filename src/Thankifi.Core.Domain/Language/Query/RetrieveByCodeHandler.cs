using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Incremental.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Core.Domain.Contract.Language.Queries;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Language.Query;

public class RetrieveByCodeHandler : QueryHandler<RetrieveByCode, LanguageDetailDto?>
{
    private readonly ThankifiDbContext _dbContext;

    public RetrieveByCodeHandler(ThankifiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<LanguageDetailDto?> Handle(RetrieveByCode request, CancellationToken cancellationToken)
    {
        var language = await _dbContext.Languages.AsNoTracking()
            .Where(l => l.Code == request.Code)
            .Select(l => new LanguageDetailDto
            {
                Id = l.Id,
                Code = l.Code,
            }).FirstOrDefaultAsync(cancellationToken);

        if (language is not null)
        {
            var query = _dbContext.Gratitudes.AsNoTracking()
                .Where(g => g.Language.Code == request.Code);

            if (request.Categories is not null && request.Categories.Any())
            {
                query = query.Where(gratitude => gratitude.Categories.Any(category => request.Categories.Any(c => c == category.Slug)));
            }

            var count = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(g => g.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(g => new GratitudeDto
                {
                    Id = g.Id,
                    Language = new LanguageDto
                    {
                        Id = g.Language.Id,
                        Code = g.Language.Code
                    },
                    Text = g.Text,
                    Categories = g.Categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Slug = c.Slug
                    })
                })
                .ToListAsync(cancellationToken);

            language = language with
            {
                Count = count,
                Gratitudes = new PaginatedList<GratitudeDto>(items, count, request.PageNumber, request.PageSize)
            };
        }

        return language;
    }
}