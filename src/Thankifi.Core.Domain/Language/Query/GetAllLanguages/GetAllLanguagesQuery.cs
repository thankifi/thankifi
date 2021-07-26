using System.Collections.Generic;
using MediatR;
using Thankifi.Core.Domain.Language.Dto;

namespace Thankifi.Core.Domain.Language.Query.GetAllLanguages
{
    public class GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>>
    {
        
    }
}