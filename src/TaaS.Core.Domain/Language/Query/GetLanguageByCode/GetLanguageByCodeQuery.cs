using MediatR;
using TaaS.Core.Domain.Language.Dto;

namespace TaaS.Core.Domain.Language.Query.GetLanguageByCode
{
    public class GetLanguageByCodeQuery : IRequest<LanguageDetailDto?>
    {
        public string Code { get; set; }
    }
}