namespace Thankifi.Core.Application.Customization
{
    public static class CustomizationHelper
    {
        public static string Customize(string text, string? subject, string? signature)
        {
            text = ReplaceSubjectIfNecessary(text, subject);
            text = AddSignatureIfNecessary(text, signature);

            return text;
        }
        
        private static string ReplaceSubjectIfNecessary(string text, string? subject)
        {
            var firstBracket = text.IndexOf('{');

            if (firstBracket == -1)
            {
                return text;
            }

            subject ??= " ";

            var lastBracket = text.LastIndexOf('}');

            var replaced = text.Substring(firstBracket, lastBracket - firstBracket + 1) switch
            {
                "{ {SUBJECT} }" => text.Replace("{ {SUBJECT} }", $" {subject} "),
                "{{SUBJECT}}" => text.Replace("{{SUBJECT}}", $"{subject}"),
                "{{SUBJECT} }" => text.Replace("{{SUBJECT} }", $"{subject} "),
                "{ {SUBJECT}}" => text.Replace("{ {SUBJECT}}", $" {subject}"),
                _ => text
            };

            return replaced;
        }

        private static string AddSignatureIfNecessary(string text, string? signature)
        {
            return signature is null ? text : $"{text} --{signature}";
        }
    }
}