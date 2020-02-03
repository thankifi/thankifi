using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Category.Query.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, (int, Entity.Category?)>
    {
        protected readonly ILogger<GetCategoryByIdQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetCategoryByIdQueryHandler(ILogger<GetCategoryByIdQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<(int, Entity.Category?)> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await Context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return (1, null);
            }

            var count = await Context.GratitudeCategories.AsNoTracking()
                .Where(gc => gc.CategoryId == request.Id)
                .CountAsync(cancellationToken);

            return (count, category);
        }
    }
}