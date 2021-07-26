using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankifi.Common.Helper;
using Thankifi.Core.Domain.Gratitude.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude
{
    public class GetBulkAllFiltersGratitudeQueryHandler : IRequestHandler<GetBulkAllFiltersGratitudeQuery, IEnumerable<GratitudeDto>>
    {
        protected readonly ILogger<GetBulkAllFiltersGratitudeQueryHandler> Logger;
        protected readonly ThankifiDbContext Context;

        public GetBulkAllFiltersGratitudeQueryHandler(ILogger<GetBulkAllFiltersGratitudeQueryHandler> logger, ThankifiDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeQuery request, CancellationToken cancellationToken)
        {
            var gratitude = new List<GratitudeDto>();

            var query = Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language);

            if (request.Category != null)
            {
                query = query.Where(g => g.Categories.Any(c => c.Category.Title.ToLower() == request.Category));
            }

            var totalGratitudeFound = await query.CountAsync(cancellationToken);

            if (request.Different)
            {
                for (var i = 0; i < 4; i++)
                {
                    var offset = RandomProvider.GetThreadRandom()?.Next(0, totalGratitudeFound);

                    gratitude.Add(await query
                        .Skip(offset ?? 0)
                        .Select(g => new GratitudeDto
                        {
                            Id = g.Id,
                            Language = g.Language,
                            Text = g.Text,
                            Categories = g.Categories.Select(gc => gc.Category.Title)
                        }).FirstOrDefaultAsync(cancellationToken));
                }
            }
            else
            {
                var offset = RandomProvider.GetThreadRandom()?.Next(0, totalGratitudeFound);

                var gratitudeDto = await query
                    .Skip(offset ?? 0)
                    .Select(g => new GratitudeDto
                    {
                        Id = g.Id,
                        Language = g.Language,
                        Text = g.Text,
                        Categories = g.Categories.Select(gc => gc.Category.Title)
                    }).FirstOrDefaultAsync(cancellationToken);

                for (var i = 0; i < 4; i++)
                {
                    gratitude.Add(new GratitudeDto
                    {
                        Id = gratitudeDto.Id,
                        Language = gratitudeDto.Language,
                        Text = gratitudeDto.Text,
                        Categories = gratitudeDto.Categories
                    });
                }
            }

            return gratitude;
        }
    }
}