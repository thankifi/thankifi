using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Entity;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeRandom
{
    public class GetGratitudeRandomQueryHandler : IRequestHandler<GetGratitudeRandomQuery, Entity.Gratitude?>
    {
        protected readonly ILogger<GetGratitudeRandomQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetGratitudeRandomQueryHandler(ILogger<GetGratitudeRandomQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<Entity.Gratitude?> Handle(GetGratitudeRandomQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random gratitude.");
            
            var offset = RandomProvider.GetThreadRandom()?.Next(0, await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language)
                .CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.AsNoTracking()
                .Include(g => g.Categories)
                    .ThenInclude(c => c.Category)
                .Where(g => g.Language == request.Language)
                .Skip(offset ?? 0)
                .FirstOrDefaultAsync(cancellationToken);

            if (gratitude != null)
            {
                gratitude.Text = gratitude.Type switch
                {
                    GratitudeType.Basic => gratitude.Text,
                    GratitudeType.Named => gratitude.Text.Replace("{{NAME}}", request.Name),
                    GratitudeType.Signed => gratitude.Text.Replace("{{SIGNATURE}}", request.Signature),
                    GratitudeType.NamedAndSigned => gratitude.Text.Replace("{{NAME}}", request.Name).Replace("{{SIGNATURE}}", request.Signature),
                    _ => gratitude.Text
                };
            }

            return gratitude;
        }
    }
}