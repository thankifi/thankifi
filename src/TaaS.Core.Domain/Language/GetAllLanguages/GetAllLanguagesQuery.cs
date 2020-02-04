using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Language.Dto;

namespace TaaS.Core.Domain.Language.GetAllLanguages
{
    public class GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>>
    {
        
    }
}