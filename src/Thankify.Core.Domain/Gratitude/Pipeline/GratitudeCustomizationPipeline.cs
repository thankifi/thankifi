using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Thankify.Core.Domain.Gratitude.Dto;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetBulkAllFiltersGratitudeById;
using Thankify.Core.Domain.Gratitude.Query.GetBulkGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetGratitude;
using Thankify.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace Thankify.Core.Domain.Gratitude.Pipeline
{
    public class GratitudeCustomizationPipeline :
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
                response.Text = ReplaceNameIfNecessary(response.Text, request.Name);

                response.Text = AddSignatureIfNecessary(response.Text, request.Signature);
            }

            return response;
        }

        public async Task<GratitudeDto?> Handle(GetGratitudeByIdQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<GratitudeDto?> next)
        {
            var response = await next();

            if (response != null)
            {
                response.Text = ReplaceNameIfNecessary(response.Text, request.Name);

                response.Text = AddSignatureIfNecessary(response.Text, request.Signature);
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
                gratitudeDtos.ForEach(dto =>
                {
                    dto.Text = ReplaceNameIfNecessary(dto.Text, request.Name);
                    dto.Text = AddSignatureIfNecessary(dto.Text, request.Signature);
                });
            }

            return gratitudeDtos;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudeDtos = response.ToList();

            if (gratitudeDtos.Any())
            {
                gratitudeDtos.ForEach(dto =>
                {
                    dto.Text = ReplaceNameIfNecessary(dto.Text, request.Name);
                    dto.Text = AddSignatureIfNecessary(dto.Text, request.Signature);
                });
            }

            return gratitudeDtos;
        }

        public async Task<IEnumerable<GratitudeDto>> Handle(GetBulkAllFiltersGratitudeByIdQuery request, CancellationToken cancellationToken,
            RequestHandlerDelegate<IEnumerable<GratitudeDto>> next)
        {
            var response = await next();

            var gratitudeDtos = response.ToList();

            if (gratitudeDtos.Any())
            {
                gratitudeDtos.ForEach(dto =>
                {
                    dto.Text = ReplaceNameIfNecessary(dto.Text, request.Name);
                    dto.Text = AddSignatureIfNecessary(dto.Text, request.Signature);
                });
            }

            return gratitudeDtos;
        }

        private static string ReplaceNameIfNecessary(string text, string? name)
        {
            var firstBracket = text.IndexOf('{');

            if (firstBracket == -1)
            {
                return text;
            }

            var lastBracket = text.LastIndexOf('}');

            var t = text.Substring(firstBracket, lastBracket - firstBracket + 1) switch
            {
                "{ {NAME} }" => text.Replace("{ {NAME} }", name != null ? $" {name} " : " "),
                "{{NAME}}" => text.Replace("{{NAME}}", name != null ? $"{name}" : ""),
                "{{NAME} }" => text.Replace("{{NAME} }", name != null ? $"{name} " : " "),
                "{ {NAME}}" => text.Replace("{ {NAME}}", name != null ? $" {name}" : ""),
                _ => text
            };

            return t;
        }

        private static string AddSignatureIfNecessary(string text, string? signature)
        {
            return signature != null ? $"{text} --{signature}" : text;
        }
    }
}