using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Query.GetGratitudeById
{
    public class GetGratitudeByIdQueryHandler : IRequestHandler<GetGratitudeByIdQuery, (int, string)>
    {
        protected readonly ILogger<GetGratitudeByIdQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetGratitudeByIdQueryHandler(ILogger<GetGratitudeByIdQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<(int, string)> Handle(GetGratitudeByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested gratitude by id.");
            
            var gratitude = await Context.Gratitudes.Where(g => g.Id == request.Id).FirstAsync(cancellationToken);

            var response = gratitude.Text.Replace("{{NAME}}", request.Name).Replace("{{SIGNATURE}}", request.Signature);

            return (gratitude.Id, response);
        }
    }
}