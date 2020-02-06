using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Domain.Gratitude.Query.GetGratitude;
using TaaS.Core.Domain.Gratitude.Query.GetGratitudeById;

namespace TaaS.Core.Domain.Gratitude.Pipeline
{
    public class GratitudeCustomizationPipeline : IPipelineBehavior<GetGratitudeQuery, GratitudeDto?>,
        IPipelineBehavior<GetGratitudeByIdQuery, GratitudeDto?>
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

        private static string ReplaceNameIfNecessary(string text, string? name)
        {
            var firstBracket = text.IndexOf('{');

            if (firstBracket == -1)
            {
                return text;
            }
            
            var lastBracket =  text.LastIndexOf('}');

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