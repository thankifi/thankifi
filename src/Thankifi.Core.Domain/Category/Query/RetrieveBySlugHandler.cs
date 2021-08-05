using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Common.Pagination;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Category.Queries;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Category.Query
{
    public class RetrieveBySlugHandler : IQueryHandler<RetrieveBySlug, CategoryDetailDto?>
    {
        private readonly ThankifiDbContext _dbContext;

        public RetrieveBySlugHandler(ThankifiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryDetailDto?> Handle(RetrieveBySlug request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories.AsNoTracking()
                .Where(c => c.Slug == request.Slug)
                .Select(c => new CategoryDetailDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                }).FirstOrDefaultAsync(cancellationToken);

            if (category is not null)
            {
                var query = _dbContext.Gratitudes.AsNoTracking()
                    .Where(g => g.Categories.Any(c => c.Slug == request.Slug));

                if (request.Languages is not null && request.Languages.Any())
                {
                    query = query.Where(gratitude => request.Languages.Any(language => language == gratitude.Language.Code));
                }

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
                    })
                    .ToListAsync(cancellationToken);

                category = category with
                {
                    Count = count,
                    Gratitudes = new PaginatedList<GratitudeDto>(items, count, request.PageNumber, request.PageSize)
                };
            }

            return category;
        }
    }
}