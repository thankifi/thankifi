using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Pagination;
using MediatR;
using Thankifi.Core.Domain.Contract.Language.Dto;
using Thankifi.Core.Domain.Contract.Language.Queries;

namespace Thankifi.Core.Application.Pipelines;

public partial class CachePipeline :
    IPipelineBehavior<RetrieveAll, PaginatedList<LanguageDto>>,
    IPipelineBehavior<RetrieveById, LanguageDetailDto?>,
    IPipelineBehavior<RetrieveByCode, LanguageDetailDto?>
{
    public async Task<PaginatedList<LanguageDto>> Handle(RetrieveAll request, CancellationToken cancellationToken,
        RequestHandlerDelegate<PaginatedList<LanguageDto>> next)
    {
        var cachedResponse = await RetrieveAsync<RetrieveAll, PaginatedList<LanguageDto>>(request, cancellationToken);

        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await next();

        await StoreAsync(request, response, cancellationToken: cancellationToken);

        return response;
    }

    public async Task<LanguageDetailDto?> Handle(RetrieveById request, CancellationToken cancellationToken,
        RequestHandlerDelegate<LanguageDetailDto?> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null && request.Categories is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveById, LanguageDetailDto?>(request, cancellationToken);

            if (cachedResponse is not null)
            {
                return cachedResponse;
            }

            var response = await next();

            await StoreAsync(request, response, cancellationToken: cancellationToken);

            return response;
        }

        return await next();
    }

    public async Task<LanguageDetailDto?> Handle(RetrieveByCode request, CancellationToken cancellationToken,
        RequestHandlerDelegate<LanguageDetailDto?> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null && request.Categories is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveByCode, LanguageDetailDto?>(request, cancellationToken);

            if (cachedResponse is not null)
            {
                return cachedResponse;
            }

            var response = await next();

            await StoreAsync(request, response, cancellationToken: cancellationToken);

            return response;
        }

        return await next();
    }
}