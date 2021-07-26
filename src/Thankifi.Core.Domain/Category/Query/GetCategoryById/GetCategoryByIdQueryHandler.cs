using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thankifi.Core.Domain.Category.Dto;
using Thankifi.Persistence.Context;

namespace Thankifi.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailDto?>
    {
        protected readonly ILogger<GetCategoryByIdQueryHandler> Logger;
        protected readonly ThankifiDbContext Context;

        public GetCategoryByIdQueryHandler(ILogger<GetCategoryByIdQueryHandler> logger, ThankifiDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<CategoryDetailDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested detailed category.");

            var query = Context.Categories.AsNoTracking()
                    .Where(c => c.Id == request.Id);

            if (request.Language != null)
            {
                query = query.Where(c => c.Gratitudes.Any(gc => gc.Gratitude.Language.ToLower() == request.Language.ToLower()));
            }
            
            var category = await query
                .Select(c => new CategoryDetailDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    TotalGratitudes = c.Gratitudes.Count
                }).FirstOrDefaultAsync(cancellationToken);

            return category;
        }
    }
}