using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Persistence.Context;

namespace Thankify.Core.Domain.Gratitude.Query.GetGratitudeById
{
    public class GetGratitudeByIdQueryHandler : IRequestHandler<GetGratitudeByIdQuery, GratitudeDto?>
    {
        protected readonly ILogger<GetGratitudeByIdQueryHandler> Logger;
        protected readonly ThankifyDbContext Context;

        public GetGratitudeByIdQueryHandler(ILogger<GetGratitudeByIdQueryHandler> logger, ThankifyDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested gratitude by id.");

            var gratitude = await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Id == request.Id)
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