namespace TaaS.Common
{
    public class CacheKeys
    {
        public static string CategoryViewModelList => "_CategoryViewModelList";
        public static string CategoryDetailViewModel(object extraIdentifier) => $"_CategoryDetailViewModel_{extraIdentifier}";
        public static string LanguageViewModelList => "_LanguageViewModelList";
    }
}