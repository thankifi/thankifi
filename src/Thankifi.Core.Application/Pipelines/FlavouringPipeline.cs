using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Thankifi.Common.Filters.Abstractions;
using Incremental.Common.Pagination;
using Thankifi.Core.Application.Customization;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Core.Application.Pipelines
{
    public class FlavouringPipeline :
        IPipelineBehavior<Domain.Contract.Category.Queries.RetrieveById, CategoryDetailDto?>,
        IPipelineBehavior<Domain.Contract.Category.Queries.RetrieveBySlug, CategoryDetailDto?>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveAll, PaginatedList<GratitudeDto>>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveById, GratitudeDto?>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveByIdFlavourful, GratitudeFlavourfulDto?>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveRandom, GratitudeDto>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveRandomBulk, IEnumerable<GratitudeDto>>,
        IPipelineBehavior<Domain.Contract.Gratitude.Queries.RetrieveRandomFlavourful, GratitudeFlavourfulDto>,
        IPipelineBehavior<Domain.Contract.Language.Queries.RetrieveById, LanguageDetailDto?>,
        IPipelineBehavior<Domain.Contract.Language.Queries.RetrieveByCode, LanguageDetailDto?>
    {
        private readonly IFilterService _filterService;

        public FlavouringPipeline(IFilterService filterService)
        {
            _filterService = filterService;
        }

        public async Task<CategoryDetailDto?> Handle(Domain.Contract.Category.Queries.RetrieveById request, CancellationToken cancellationToken,
            RequestHandlerDelegate<CategoryDetailDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }

            var flavoured = new List<GratitudeDto>(response.Gratitudes.Capacity);

            foreach (var gratitude in response.Gratitudes)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            var gratitudes = new PaginatedList<GratitudeDto>(flavoured, response.Gratitudes.Count, response.Gratitudes.CurrentPage,
                response.Gratitudes.PageSize);

            return response with
            {
                Gratitudes = gratitudes
            };
        }

        public async Task<CategoryDetailDto?> Handle(Domain.Contract.Category.Queries.RetrieveBySlug request, CancellationToken cancellationToken,
            RequestHandlerDelegate<CategoryDetailDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }

            var flavoured = new List<GratitudeDto>(response.Gratitudes.Capacity);

            foreach (var gratitude in response.Gratitudes)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            var gratitudes = new PaginatedList<GratitudeDto>(flavoured, response.Gratitudes.Count, response.Gratitudes.CurrentPage,
                response.Gratitudes.PageSize);

            return response with
            {
                Gratitudes = gratitudes
            };
        }

        public async Task<PaginatedList<GratitudeDto>> Handle(Domain.Contract.Gratitude.Queries.RetrieveAll request, CancellationToken cancellationToken, RequestHandlerDelegate<PaginatedList<GratitudeDto>> next)
        {
            var response = await next();

            if (response.Count is 0)
            {
                return response;
            }
            
            var flavoured = new List<GratitudeDto>(response.Capacity);

            foreach (var gratitude in response)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            return new PaginatedList<GratitudeDto>(flavoured, response.Count, response.CurrentPage, response.PageSize);
        }

        public async Task<GratitudeDto?> Handle(Domain.Contract.Gratitude.Queries.RetrieveById request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }
            
            var text = CustomizationHelper.Customize(response.Text, request.Subject, request.Signature);

            return response with
            {
                Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
            };
        }

        public async Task<GratitudeFlavourfulDto?> Handle(Domain.Contract.Gratitude.Queries.RetrieveByIdFlavourful request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeFlavourfulDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }
            
            var text = CustomizationHelper.Customize(response.Text, request.Subject, request.Signature);

            var availableFlavours = _filterService.GetAvailableFilterIdentifiers();

            var flavours = new List<FlavourDto>();
            
            foreach (var availableFlavour in availableFlavours)
            {
                var flavour = new FlavourDto
                {
                    Flavour = availableFlavour,
                    Text = await _filterService.Apply(availableFlavour, text, cancellationToken)
                };
                
                flavours.Add(flavour);
            }
            
            return response with
            {
                Text = text,
                Flavours = flavours
            };
        }

        public async Task<GratitudeDto> Handle(Domain.Contract.Gratitude.Queries.RetrieveRandom request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeDto> next)
        {
            var response = await next();

            var text = CustomizationHelper.Customize(response.Text, request.Subject, request.Signature);

            return response with
            {
                Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
            };
        }
        public async Task<IEnumerable<GratitudeDto>> Handle(Domain.Contract.Gratitude.Queries.RetrieveRandomBulk request, CancellationToken cancellationToken, RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudes = response.ToList();

            if (gratitudes.Count() is 0)
            {
                return gratitudes;
            }
            
            var flavoured = new List<GratitudeDto>(gratitudes.Capacity);

            foreach (var gratitude in gratitudes)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            return flavoured;
        }

        public async Task<GratitudeFlavourfulDto> Handle(Domain.Contract.Gratitude.Queries.RetrieveRandomFlavourful request, CancellationToken cancellationToken, RequestHandlerDelegate<GratitudeFlavourfulDto> next)
        {
            var response = await next();

            var text = CustomizationHelper.Customize(response.Text, request.Subject, request.Signature);

            var availableFlavours = _filterService.GetAvailableFilterIdentifiers();

            var flavours = new List<FlavourDto>();
            
            foreach (var availableFlavour in availableFlavours)
            {
                var flavour = new FlavourDto
                {
                    Flavour = availableFlavour,
                    Text = await _filterService.Apply(availableFlavour, text, cancellationToken)
                };
                
                flavours.Add(flavour);
            }
            
            return response with
            {
                Text = text,
                Flavours = flavours
            };

        }

        public async Task<LanguageDetailDto?> Handle(Domain.Contract.Language.Queries.RetrieveById request, CancellationToken cancellationToken, RequestHandlerDelegate<LanguageDetailDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }

            var flavoured = new List<GratitudeDto>(response.Gratitudes.Capacity);

            foreach (var gratitude in response.Gratitudes)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            var gratitudes = new PaginatedList<GratitudeDto>(flavoured, response.Gratitudes.Count, response.Gratitudes.CurrentPage,
                response.Gratitudes.PageSize);

            return response with
            {
                Gratitudes = gratitudes
            };
        }

        public async Task<LanguageDetailDto?> Handle(Domain.Contract.Language.Queries.RetrieveByCode request, CancellationToken cancellationToken, RequestHandlerDelegate<LanguageDetailDto?> next)
        {
            var response = await next();

            if (response is null)
            {
                return response;
            }

            var flavoured = new List<GratitudeDto>(response.Gratitudes.Capacity);

            foreach (var gratitude in response.Gratitudes)
            {
                var text = CustomizationHelper.Customize(gratitude.Text, request.Subject, request.Signature);

                flavoured.Add(gratitude with
                {
                    Text = await _filterService.ApplyOrDefault(request.Flavours ?? Array.Empty<string>(), text, cancellationToken) ?? text
                });
            }

            var gratitudes = new PaginatedList<GratitudeDto>(flavoured, response.Gratitudes.Count, response.Gratitudes.CurrentPage,
                response.Gratitudes.PageSize);

            return response with
            {
                Gratitudes = gratitudes
            };
        }
    }
}