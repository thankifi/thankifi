using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankify.Common.Helper;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Persistence.Context;

namespace Thankify.Core.Domain.Gratitude.Query.GetBulkGratitude
{
    public class GetBulkGratitudeQueryHandler : IRequestHandler<GetBulkGratitudeQuery, IEnumerable<GratitudeDto>>
    {
        protected readonly ILogger<GetBulkGratitudeQueryHandler> Logger;
        protected readonly ThankifyDbContext Context;

        public GetBulkGratitudeQueryHandler(ILogger<GetBulkGratitudeQueryHandler> logger, ThankifyDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkGratitudeQuery request, CancellationToken cancellationToken)
        {
            var gratitude = new List<GratitudeDto>();

            var query = Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language);

            if (request.Category != null)
            {
                query = query.Where(g => g.Categories.Any(c => c.Category.Title.ToLower() == request.Category));
            }

            var totalGratitudeFound = await query.CountAsync(cancellationToken);

            for (var i = 0; i < request.Quantity; i++)
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

            return gratitude;
        }
    }
}