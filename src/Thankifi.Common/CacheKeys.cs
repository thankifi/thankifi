namespace Thankifi.Common
{
    public class CacheKeys
    {
        public static string CategoryViewModelList(object language) => $"_CategoryViewModelList_{language}";
        public static string CategoryDetailViewModel(object categoryIdentifier, object? language) => language != null ? $"_CategoryDetailViewModel_{categoryIdentifier}_{language}" : $"_CategoryDetailViewModel_{categoryIdentifier}";
        public static string LanguageViewModelList => "_LanguageViewModelList";
        public static string LanguageDetailViewModel(object languageIdentifier) => $"_LanguageDetailViewModel_{languageIdentifier}";
    }
}