using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Category.Query.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<Entity.Category>>
    {
        protected readonly ILogger<GetAllCategoriesQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetAllCategoriesQueryHandler(ILogger<GetAllCategoriesQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<IEnumerable<Entity.Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Requested categories list.");

            var categories = await Context.Categories.AsNoTracking().ToListAsync(cancellationToken);

            return categories;
        }
    }
}