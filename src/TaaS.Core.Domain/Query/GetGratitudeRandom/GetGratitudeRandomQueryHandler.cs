using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Query.GetGratitudeRandom
{
    public class GetRandomGratitudeQueryHandler : IRequestHandler<GetRandomGratitudeQuery, (int, string)>
    {
        protected readonly ILogger<GetRandomGratitudeQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetRandomGratitudeQueryHandler(ILogger<GetRandomGratitudeQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<(int, string)> Handle(GetRandomGratitudeQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random gratitude.");
            
            var offset = RandomProvider.GetThreadRandom().Next(0, await Context.Gratitudes.CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.Skip(offset).FirstAsync(cancellationToken: cancellationToken);

            var response = gratitude.Text.Replace("{{NAME}}", request.Name).Replace("{{SIGNATURE}}", request.Signature);

            return (gratitude.Id, response);
        }
    }
}