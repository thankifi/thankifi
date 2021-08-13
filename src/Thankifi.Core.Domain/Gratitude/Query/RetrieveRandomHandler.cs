using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Common.Random;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Gratitude.Query
{
    public class RetrieveRandomHandler : IQueryHandler<RetrieveRandom, GratitudeDto>
    {
        private readonly ThankifiDbContext _dbContext;

        public RetrieveRandomHandler(ThankifiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GratitudeDto> Handle(RetrieveRandom request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Gratitudes.AsNoTracking();

            if (request.Languages is not null && request.Languages.Any())
            {
                query = query.Where(gratitude => request.Languages.Any(language => language == gratitude.Language.Code));
            }

            if (request.Categories is not null && request.Categories.Any())
            {
                query = query.Where(gratitude => gratitude.Categories.Any(category => request.Categories.Any(c => c == category.Slug)));
            }

            var offset = RandomProvider.GetThreadRandom()?.Next(0, await query.CountAsync(cancellationToken));

            var gratitude = await query
                .Skip(offset ?? 0)
                .Select(g => new GratitudeDto
                {
                    Id = g.Id,
                    Language = new LanguageDto
                    {
                        Id = g.Language.Id,
                        Code = g.Language.Code
                    },
                    Text = g.Text,
                    Categories = g.Categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Slug = c.Slug
                    })
                })
                .FirstOrDefaultAsync(cancellationToken);
            
            return gratitude;
        }
    }
}