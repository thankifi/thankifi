using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Pagination;
using MediatR;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Queries;

namespace Thankifi.Core.Application.Pipelines;

public partial class CachePipeline:
    IPipelineBehavior<RetrieveAll, PaginatedList<GratitudeDto>>,
    IPipelineBehavior<RetrieveById, GratitudeDto?>,
    IPipelineBehavior<RetrieveByIdFlavourful, GratitudeFlavourfulDto?>
{
    public async Task<PaginatedList<GratitudeDto>> Handle(RetrieveAll request, CancellationToken cancellationToken, RequestHandlerDelegate<PaginatedList<GratitudeDto>> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveAll, PaginatedList<GratitudeDto>>(request, cancellationToken);

            if (cachedResponse is null)
            {
                var response = await next();

                await StoreAsync(request, response, cancellationToken: cancellationToken);

                return response;
            }

            return cachedResponse;
        }

        return await next();
    }

    public async Task<GratitudeDto?> Handle(RetrieveById request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeDto?> next)
    {
        if (request.Subject is null && request.Signature is null && request.Flavours is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveById, GratitudeDto?>(request, cancellationToken);

            if (cachedResponse is null)
            {
                var response = await next();

                await StoreAsync(request, response, cancellationToken: cancellationToken);

                return response;
            }

            return cachedResponse;
        }

        return await next();
    }

    public async Task<GratitudeFlavourfulDto?> Handle(RetrieveByIdFlavourful request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeFlavourfulDto?> next)
    {
        if (request.Subject is null && request.Signature is null)
        {
            var cachedResponse = await RetrieveAsync<RetrieveByIdFlavourful, GratitudeFlavourfulDto?>(request, cancellationToken);

            if (cachedResponse is null)
            {
                var response = await next();

                await StoreAsync(request, response, cancellationToken: cancellationToken);

                return response;
            }

            return cachedResponse;
        }

        return await next();
    }
}