using System.Collections.Generic;
using MediatR;
using Thankify.Core.Domain.Language.Dto;

namespace Thankify.Core.Domain.Language.Query.GetAllLanguages
{
    public class GetAllLanguagesQuery : IRequest<IEnumerable<LanguageDto>>
    {
        
    }
}