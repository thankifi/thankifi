using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Thankifi.Api.Configuration.Swagger
{
    public class AnalyticsTagHelperComponent : TagHelperComponent
    {
        private readonly AnalyticsTagHelperOptions _analyticsOptions;

        public AnalyticsTagHelperComponent(IOptions<AnalyticsTagHelperOptions> analyticsTagHelperOptions)
        {
            _analyticsOptions = analyticsTagHelperOptions.Value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!output.TagName.Equals("head", StringComparison.OrdinalIgnoreCase)) return;
            
            if (!string.IsNullOrWhiteSpace(_analyticsOptions.GTagTrackingCode))
            {
                output.PostContent.AppendHtml(
                    $"<script async src=\"https://www.googletagmanager.com/gtag/js?id={_analyticsOptions.GTagTrackingCode}\"></script><script>window.dataLayer = window.dataLayer || []; function gtag(){{dataLayer.push(arguments);}} gtag('js', new Date()); gtag('config', '{_analyticsOptions.GTagTrackingCode}');</script>");
            }

            if (!string.IsNullOrWhiteSpace(_analyticsOptions.PlausibleTrackingCode) && string.IsNullOrWhiteSpace(_analyticsOptions.PlausibleScriptSource))
            {
                output.PostContent.AppendHtml(
                    $"<script defer data-domain=\"{_analyticsOptions.PlausibleTrackingCode}\" src=\"{_analyticsOptions.PlausibleScriptSource}\"></script>");;
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