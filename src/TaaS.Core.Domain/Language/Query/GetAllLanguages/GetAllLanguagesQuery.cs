using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Language.Dto;

namespace TaaS.Core.Domain.Language.Query.GetAllLanguages
{
    public class GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>>
    {
        
    }
}