using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Core.Domain.Category.Dto;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailDto?>
    {
        protected readonly ILogger<GetCategoryByIdQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetCategoryByIdQueryHandler(ILogger<GetCategoryByIdQueryHandler> logger, TaaSDbContext context)
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
                query = query.Where(c => c.Gratitudes.Any(gc => gc.Gratitude.Language.ToLower() == request.Language));
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