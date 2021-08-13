using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Gratitude.Query
{
    public class RetrieveByIdFlavourfulHandler : IQueryHandler<RetrieveByIdFlavourful, GratitudeFlavourfulDto>
    {
        private readonly ThankifiDbContext _dbContext;

        public async Task<GratitudeFlavourfulDto> Handle(RetrieveByIdFlavourful request, CancellationToken cancellationToken)
        {
            var gratitude = await _dbContext.Gratitudes.AsNoTracking()
                .Where(g => g.Id == request.Id)
                .Select(g => new GratitudeFlavourfulDto
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