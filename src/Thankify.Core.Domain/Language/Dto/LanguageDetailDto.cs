namespace Thankify.Core.Domain.Language.Dto
{
    public class LanguageDetailDto : LanguageDto
    {
        public LanguageDetailDto(string code, int totalGratitudes) : base(code)
        {
            TotalGratitudes = totalGratitudes;
        }
        
        public int TotalGratitudes { get; set; }
    }
}