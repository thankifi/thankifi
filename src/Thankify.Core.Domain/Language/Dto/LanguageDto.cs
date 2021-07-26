namespace Thankify.Core.Domain.Language.Dto
{
    public class LanguageDto
    {
        public LanguageDto(string code)
        {
            Code = code;
            Reference = $"https://iso639-3.sil.org/code/{code}";
        }

        public string Code { get; }
        public string Reference { get; }
    }
}