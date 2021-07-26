using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Thankifi.Common.Filter;
using Thankifi.Core.Domain.Gratitude.Dto;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using Thankifi.Core.Domain.Gratitude.Query.GetBulkGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetGratitude;
using Thankifi.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace Thankifi.Core.Domain.Gratitude.Pipeline
{
    public class GratitudeFilterPipeline :
        IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>,
        IPipelineBehavior<GetGratitudeByIdQuery, GratitudeDto?>,
        IPipelineBehavior<GetBulkGratitudeQuery, IEnumerable<GratitudeDto>>,
        IPipelineBehavior<GetBulkAllFiltersGratitudeQuery, IEnumerable<GratitudeDto>>,
        IPipelineBehavior<GetBulkAllFiltersGratitudeByIdQuery, IEnumerable<GratitudeDto>>
    {
        public async Task<GratitudeDto?> Handle(GetGratitudeQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<GratitudeDto?> next)
        {
            var response = await next();

            if (response != null)
            {
                response.Text = ApplyFilters(response.Text, request.Filters);
            }

            return response;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeByIdQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<GratitudeDto?> next)
        {
            var response = await next();

            if (response != null)
            {
                response.Text = ApplyFilters(response.Text, request.Filters);
            }

            return response;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkGratitudeQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudeDtos = response.ToList();
            
            if (gratitudeDtos.Any())
            {
                gratitudeDtos.ForEach(dto => dto.Text = ApplyFilters(dto.Text, request.Filters));
            }

            return gratitudeDtos;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudeDtos = response.ToList();
            
            if (gratitudeDtos.Count == 4)
            {
                gratitudeDtos[1].Text = ApplyFilters(gratitudeDtos[1].Text, new[] {"mocking"});
                gratitudeDtos[2].Text = ApplyFilters(gratitudeDtos[2].Text, new[] {"shouting"});
                gratitudeDtos[3].Text = ApplyFilters(gratitudeDtos[3].Text, new[] {"leet"});
            }

            return gratitudeDtos;
        }
        
        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeByIdQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudeDtos = response.ToList();
            
            if (gratitudeDtos.Count == 4)
            {
                var text = gratitudeDtos[0].Text;
                
                gratitudeDtos[1].Text = ApplyFilters(text, new[] {"mocking"});
                gratitudeDtos[2].Text = ApplyFilters(text, new[] {"shouting"});
                gratitudeDtos[3].Text = ApplyFilters(text, new[] {"leet"});
            }

            return gratitudeDtos;
        }
        
        private static string ApplyFilters(string text, IEnumerable<string> requestFilters)
        {
            return requestFilters.Aggregate(text, (current, filter) => filter switch
            {
                "mocking" => MockFilter.Apply(current),
                "shouting" => current.ToUpper(),
                "leet" => LeetFilter.Apply(current),
                _ => current
            });
        }
    }
}