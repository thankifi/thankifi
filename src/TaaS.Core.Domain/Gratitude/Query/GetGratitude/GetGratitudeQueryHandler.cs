using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitude
{
    public class GetGratitudeQueryHandler : IRequestHandler<GetGratitudeQuery, GratitudeDto?>
    {
        protected readonly ILogger<GetGratitudeQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetGratitudeQueryHandler(ILogger<GetGratitudeQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeQuery request, CancellationToken cancellationToken)
        {
            var offset = RandomProvider.GetThreadRandom()?.Next(0, await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language)
                .CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language)
                .Skip(offset ?? 0)
                .Select(g => new GratitudeDto
                {
                    Id = g.Id,
                    Language = g.Language,
                    Text = g.Text,
                    Categories = g.Categories.Select(gc => gc.Category.Title)
                }).FirstOrDefaultAsync(cancellationToken);

            return gratitude;
        }
    }
}