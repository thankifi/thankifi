using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankify.Common.Helper;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Persistence.Context;

namespace Thankify.Core.Domain.Gratitude.Query.GetGratitude
{
    public class GetGratitudeQueryHandler : IRequestHandler<GetGratitudeQuery, GratitudeDto?>
    {
        protected readonly ILogger<GetGratitudeQueryHandler> Logger;
        protected readonly ThankifyDbContext Context;

        public GetGratitudeQueryHandler(ILogger<GetGratitudeQueryHandler> logger, ThankifyDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeQuery request, CancellationToken cancellationToken)
        {
            var query = Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language);

            if (request.Category != null)
            {
                query = query.Where(g => g.Categories.Any(c => c.Category.Title.ToLower() == request.Category));
            }
            
            var offset = RandomProvider.GetThreadRandom()?.Next(0, await query.CountAsync(cancellationToken));

            var gratitude = await query
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