using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Domain.Gratitude.Query.GetGratitude;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeByCategory;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace TaaS.Core.Domain.Gratitude.Pipeline
{
    public class GratitudeFilterPipeline : IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>,
        IPipelineBehavior<GetGratitudeByCategoryQuery, GratitudeDto?>,
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

        public async Task<GratitudeDto?> Handle(GetGratitudeByCategoryQuery request, CancellationToken cancellationToken,
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
                "mocking" => Mock(current),
                "shouting" => current.ToUpper(),
                _ => current
            });
        }

        private static string Mock(string text)
        {
            var lastIsUpper = true;
            var stringBuilder = new StringBuilder(text.Length);

            foreach (var character in text)
            {
                if (char.IsLetter(character))
                {
                    stringBuilder.Append(lastIsUpper ? char.ToLower(character) : char.ToUpper(character));
                    lastIsUpper = !lastIsUpper;
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }
    }
}