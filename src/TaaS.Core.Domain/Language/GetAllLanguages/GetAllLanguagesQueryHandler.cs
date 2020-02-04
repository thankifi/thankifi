using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaaS.Core.Domain.Language.Dto;
using TaaS.Persistence.Context;

namespace TaaS.Core.Domain.Language.GetAllLanguages
{
    public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<LanguageDto>>
    {
        protected readonly ILogger<GetAllLanguagesQueryHandler> Logger;
        protected readonly TaaSDbContext Context;

        public GetAllLanguagesQueryHandler(ILogger<GetAllLanguagesQueryHandler> logger, TaaSDbContext context)
        {
            Logger = logger;
            Context = context;
        }

        public async Task<IEnumerable<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            Logger.LogDebug("Language list requested");
            
            var languages = await Context.Gratitudes.AsNoTracking()
                .Select(g => new LanguageDto(g.Language))
                .Distinct()
                .ToListAsync(cancellationToken);

            return languages;
        }
    }
}