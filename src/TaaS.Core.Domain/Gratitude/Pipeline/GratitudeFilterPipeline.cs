using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TaaS.Common.Filter;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Domain.Gratitude.Query.GetGratitude;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace TaaS.Core.Domain.Gratitude.Pipeline
{
    public class GratitudeFilterPipeline : IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>,
        IPipelineBehavior<GetGratitudeByIdQuery, GratitudeDto?>
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