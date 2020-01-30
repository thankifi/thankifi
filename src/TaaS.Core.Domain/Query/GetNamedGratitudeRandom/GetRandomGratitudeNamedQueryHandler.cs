using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Entity;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Query.GetNamedGratitudeRandom
{
    public class GetRandomGratitudeNamedQueryHandler : IRequestHandler<GetRandomGratitudeNamedQuery, (int, string)>
    {
        protected readonly ILogger<GetRandomGratitudeNamedQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetRandomGratitudeNamedQueryHandler(ILogger<GetRandomGratitudeNamedQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<(int, string)> Handle(GetRandomGratitudeNamedQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random basic gratitude.");
            
            var offset = RandomProvider.GetThreadRandom().Next(0, await Context.Gratitudes.Where(g => g.Type == GratitudeType.Basic).CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.Where(g => g.Type == GratitudeType.Named).Skip(offset).FirstAsync(cancellationToken);

            var response = gratitude.Text.Replace("{{NAME}}", request.Name);

            return (gratitude.Id, response);
        }
    }
}