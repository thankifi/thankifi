using TaaS.Core.Domain.Language.Dto;

namespace TaaS.Api.WebApi.Model.V1
{
    public class LanguageDetailViewModel : LanguageViewModel
    {
        public int TotalGratitudes { get; set; }
        
        public static LanguageDetailViewModel Parse(LanguageDetailDto languageDetailDto)
        {
            return new LanguageDetailViewModel
            {
                Code = languageDetailDto.Code,
                Reference = languageDetailDto.Reference,
                TotalGratitudes = languageDetailDto.TotalGratitudes
            };
        }
    }
}