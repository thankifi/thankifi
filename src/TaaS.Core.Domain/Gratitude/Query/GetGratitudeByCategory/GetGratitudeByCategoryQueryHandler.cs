using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Common.Helper;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Entity;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeByCategory
{
    public class GetGratitudeByCategoryQueryHandler : IRequestHandler<GetGratitudeByCategoryQuery, GratitudeDto?>
    {
        protected readonly ILogger<GetGratitudeByCategoryQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetGratitudeByCategoryQueryHandler(ILogger<GetGratitudeByCategoryQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeByCategoryQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested random gratitude by category.");

            var offset = RandomProvider.GetThreadRandom()?.Next(0, await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language)
                .Where(g => g.Categories.Any(c => c.CategoryId == request.CategoryId || c.Category.Title == request.CategoryTitle))
                .CountAsync(cancellationToken));

            var gratitude = await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Language == request.Language)
                .Where(g => g.Categories.Any(c => c.CategoryId == request.CategoryId || c.Category.Title == request.CategoryTitle))
                .Skip(offset ?? 0)
                .Select(g => new GratitudeDto
                {
                    Id = g.Id,
                    Language = g.Language,
                    Text = g.Text,
                    Customization = (int) g.Type,
                    Categories = g.Categories.Select(gc => gc.Category.Title)
                }).FirstOrDefaultAsync(cancellationToken);


            if (gratitude != null)
            {
                gratitude.Text = gratitude.Customization switch
                {
                    (int) GratitudeType.Basic => gratitude.Text,
                    (int) GratitudeType.Named => gratitude.Text.Replace("{{NAME}}", request.Name),
                    (int) GratitudeType.Signed => gratitude.Text.Replace("{{SIGNATURE}}", request.Signature),
                    (int) GratitudeType.NamedAndSigned => gratitude.Text.Replace("{{NAME}}", request.Name)
                        .Replace("{{SIGNATURE}}", request.Signature),
                    _ => gratitude.Text
                };
            }

            return gratitude;        }
    }
}