using System;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Pagination;
using MediatR;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Category.Queries;

namespace Thankifi.Core.Application.Pipelines;

public partial class CachePipeline : 
    IPipelineBehavior<RetrieveAll, PaginatedList<CategoryDto>>,
    IPipelineBehavior<RetrieveById, CategoryDetailDto?>,
    IPipelineBehavior<RetrieveBySlug, CategoryDetailDto?>
{
    public async Task<PaginatedList<CategoryDto>> Handle(RetrieveAll request, CancellationToken cancellationToken,
        RequestHandlerDelegate<PaginatedList<CategoryDto>> next)
    {
        var cachedResponse = await RetrieveAsync<RetrieveAll, PaginatedList<CategoryDto>>(request, cancellationToken);

        if (cachedResponse is null)
        {
            var response = await next();

            await StoreAsync(request, response, cancellationToken: cancellationToken);

            return response;
        }

        return cachedResponse;
    }

    public async Task<CategoryDetailDto?> Handle(RetrieveById request, CancellationToken cancellationToken,
        RequestHandlerDelegate<CategoryDetailDto?> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null && request.Languages is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveById, CategoryDetailDto?>(request, cancellationToken);

            if (cachedResponse is null)
            {
                var response = await next();

                await StoreAsync(request, response, cancellationToken: cancellationToken);
            }
            
            return cachedResponse;
        }

        return await next();
    }

    public async Task<CategoryDetailDto?> Handle(RetrieveBySlug request, CancellationToken cancellationToken,
        RequestHandlerDelegate<CategoryDetailDto?> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null && request.Languages is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveBySlug, CategoryDetailDto?>(request, cancellationToken);

            if (cachedResponse is null)
            {
                var response = await next();

                await StoreAsync(request, response, cancellationToken: cancellationToken);
            }
            
            return cachedResponse;
        }

        return await next();
    }
}