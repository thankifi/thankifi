namespace TaaS.Api.WebApi.Configuration.Swagger
{
    public static class GAnalyticsHeader
    {
        public static string GetHeader(string gId)
        {
            return
                $"<!-- Global site tag (gtag.js) - Google Analytics -->\n<script async src=\"https://www.googletagmanager.com/gtag/js?id={gId}\"></script>\n<script>\n  window.dataLayer = window.dataLayer || [];\n  function gtag(){{dataLayer.push(arguments);}}\n  gtag('js', new Date());\n\n  gtag('config', '{gId}');\n</script>\n";
        }
    }
}