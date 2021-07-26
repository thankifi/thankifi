using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Persistence.Context;

namespace Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById
{
    public class GetBulkAllFiltersGratitudeByIdQueryHandler : IRequestHandler<GetBulkAllFiltersGratitudeByIdQuery, IEnumerable<GratitudeDto>>
    {
        protected readonly ILogger<GetBulkAllFiltersGratitudeByIdQueryHandler> Logger;
        protected readonly ThankifyDbContext Context;

        public GetBulkAllFiltersGratitudeByIdQueryHandler(ILogger<GetBulkAllFiltersGratitudeByIdQueryHandler> logger, ThankifyDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeByIdQuery request, CancellationToken cancellationToken)
        {
            var gratitude = new List<GratitudeDto>();
            
            var gratitudeDto = await Context.Gratitudes.AsNoTracking()
                .Where(g => g.Id == request.Id)
                .Select(g => new GratitudeDto
                {
                    Id = g.Id,
                    Language = g.Language,
                    Text = g.Text,
                    Categories = g.Categories.Select(gc => gc.Category.Title)
                }).FirstOrDefaultAsync(cancellationToken);

            for (var i = 0; i < 4; i++)
            {
                gratitude.Add(new GratitudeDto
                {
                    Id = gratitudeDto.Id,
                    Language = gratitudeDto.Language,
                    Text = gratitudeDto.Text,
                    Categories = gratitudeDto.Categories
                });
            }

            return gratitude;
        }
    }
}