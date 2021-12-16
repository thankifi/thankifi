using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Thankifi.Api.Configuration.Swagger
{
    public static class AnalyticsHeadContent
    {
        private static readonly AnalyticsTagHelperOptions AnalyticsOptions;
        public static string Content { get; private set; }
        
        static AnalyticsHeadContent()
        {
            AnalyticsOptions = new AnalyticsTagHelperOptions();
            Content = string.Empty;
        }
        public static void Configure(IConfiguration configuration)
        {
            AnalyticsOptions.GTagTrackingCode = configuration["GTAG_CODE"];
            AnalyticsOptions.PlausibleTrackingCode = configuration["PLAUSIBLE_CODE"];
            AnalyticsOptions.PlausibleScriptSource = configuration["PLAUSIBLE_SOURCE"];
            
            
            if (!string.IsNullOrWhiteSpace(AnalyticsOptions.GTagTrackingCode))
            {
                Content += 
                    $"<script async src=\"https://www.googletagmanager.com/gtag/js?id={AnalyticsOptions.GTagTrackingCode}\"></script><script>window.dataLayer = window.dataLayer || []; function gtag(){{dataLayer.push(arguments);}} gtag('js', new Date()); gtag('config', '{AnalyticsOptions.GTagTrackingCode}');</script>";
            }

            if (!string.IsNullOrWhiteSpace(AnalyticsOptions.PlausibleTrackingCode) && string.IsNullOrWhiteSpace(AnalyticsOptions.PlausibleScriptSource))
            {
                Content +=
                    $"<script defer data-domain=\"{AnalyticsOptions.PlausibleTrackingCode}\" src=\"{AnalyticsOptions.PlausibleScriptSource}\"></script>";
            }
        }
            
        public class AnalyticsTagHelperOptions
        {
            public string GTagTrackingCode { get; set; }
            public string PlausibleTrackingCode { get; set; }
            public string PlausibleScriptSource { get; set; }
        }
    }
}