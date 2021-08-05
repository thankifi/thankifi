using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Gratitude.Query
{
    public class RetrieveAllHandler : IQueryHandler<RetrieveAll, PaginatedList<GratitudeDto>>
    {
        private readonly ThankifiDbContext _dbContext;

        public RetrieveAllHandler(ThankifiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<GratitudeDto>> Handle(RetrieveAll request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Gratitudes.AsNoTracking();

            var count = await query.CountAsync(cancellationToken);

            var items = await query
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
                }).ToListAsync(cancellationToken);

            return new PaginatedList<GratitudeDto>(items, count, request.PageNumber, request.PageSize);
        }
    }
}