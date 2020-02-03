using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Entity;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Query.GetGratitudeRandomByType
{
    public class GetGratitudeRandomByTypeQueryHandler : IRequestHandler<GetGratitudeRandomByTypeQuery, Entity.Gratitude>
    {
        protected readonly ILogger<GetGratitudeRandomByTypeQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetGratitudeRandomByTypeQueryHandler(ILogger<GetGratitudeRandomByTypeQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<Entity.Gratitude> Handle(GetGratitudeRandomByTypeQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random basic gratitude.");
            
            var offset = RandomProvider.GetThreadRandom().Next(0, await Context.Gratitudes
                .Where(g => g.Language == request.Language)
                .Where(g => g.Type == request.Type)
                .CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes
                .Include(g => g.Categories)
                    .ThenInclude(c => c.Category)
                .Where(g => g.Language == request.Language)
                .Where(g => g.Type == request.Type)
                .Skip(offset)
                .FirstAsync(cancellationToken);

            gratitude.Text = gratitude.Type switch
            {
                GratitudeType.Basic => gratitude.Text,
                GratitudeType.Named => gratitude.Text.Replace("{{NAME}}", request.Name),
                GratitudeType.Signed => gratitude.Text.Replace("{{SIGNATURE}}", request.Signature),
                GratitudeType.NamedAndSigned => gratitude.Text.Replace("{{NAME}}", request.Name).Replace("{{SIGNATURE}}", request.Signature),
                _ => gratitude.Text
            };

            return gratitude;
        }
    }
}