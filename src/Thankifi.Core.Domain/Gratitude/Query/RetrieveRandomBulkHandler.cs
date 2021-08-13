using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Common.Random;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Gratitude.Query
{
    public class RetrieveRandomBulkHandler : IQueryHandler<RetrieveRandomBulk, IEnumerable<GratitudeDto>>
    {
        private readonly ThankifiDbContext _dbContext;

        public RetrieveRandomBulkHandler(ThankifiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(RetrieveRandomBulk request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Gratitudes.AsNoTracking();

            if (request.Languages is not null && request.Languages.Any())
            {
                query = query.Where(g => request.Languages.Any(language => language == g.Language.Code));
            }

            if (request.Categories is not null && request.Categories.Any())
            {
                query = query.Where(g => g.Categories.Any(category => request.Categories.Any(c => c == category.Slug)));
            }

            var totalGratitudes = await query.CountAsync(cancellationToken);

            var gratitudes = new List<GratitudeDto>(request.Quantity);

            while (gratitudes.Count < totalGratitudes && gratitudes.Count < request.Quantity)
            {
                var offset = RandomProvider.GetThreadRandom()?.Next(0, totalGratitudes);

                gratitudes.Add(await query
                    .OrderBy(g => g.Id)
                    .Skip(offset ?? 0)
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
                    }).FirstOrDefaultAsync(cancellationToken));
            }

            return gratitudes;
        }
    }
}