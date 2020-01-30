using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Entity;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Query.GetBasicGratitudeRandom
{
    public class GetRandomGratitudeBasicQueryHandler : IRequestHandler<GetRandomGratitudeBasicQuery, (int, string)>
    {
        protected readonly ILogger<GetRandomGratitudeBasicQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetRandomGratitudeBasicQueryHandler(ILogger<GetRandomGratitudeBasicQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<(int, string)> Handle(GetRandomGratitudeBasicQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random basic gratitude.");
            
            var offset = RandomProvider.GetThreadRandom().Next(0, await Context.Gratitudes.Where(g => g.Type == GratitudeType.Basic).CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.Where(g => g.Type == GratitudeType.Basic).Skip(offset).FirstAsync(cancellationToken);

            var response = gratitude.Text;

            return (gratitude.Id, response);
        }
    }
}