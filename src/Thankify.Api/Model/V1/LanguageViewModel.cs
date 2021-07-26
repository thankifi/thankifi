using System.Collections.Generic;
using System.Linq;
using Thankify.Core.Domain.Language.Dto;

namespace Thankify.Api.Model.V1
{
    public class LanguageViewModel
    {
        public string Code { get; set; }
        public string Reference { get; set; }

        public static LanguageViewModel Parse(LanguageDto languageDto)
        {
            return new LanguageViewModel
            {
                Code = languageDto.Code,
                Reference = languageDto.Reference
            };
        }
        
        public static IEnumerable<LanguageViewModel> Parse(IEnumerable<LanguageDto> languageDtos)
        {
            return languageDtos.Select(Parse);
        }
    }
}