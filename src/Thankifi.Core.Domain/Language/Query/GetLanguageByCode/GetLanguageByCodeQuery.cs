using MediatR;
using Thankifi.Core.Domain.Language.Dto;

namespace Thankifi.Core.Domain.Language.Query.GetLanguageByCode
{
    public class GetLanguageByCodeQuery : IRequest<LanguageDetailDto?>
    {
        public string Code { get; set; }
    }
}