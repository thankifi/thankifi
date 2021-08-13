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

            var lastBracket = text.LastIndexOf('}');

            return subject is null
                ? text.Substring(firstBracket, lastBracket - firstBracket + 1) switch
                {
                    "{ {SUBJECT} }" => text.Replace("{ {SUBJECT} }", " "),
                    "{{SUBJECT}}" => text.Replace("{{SUBJECT}}", string.Empty),
                    "{{SUBJECT} }" => text.Replace("{{SUBJECT} }", " "),
                    "{ {SUBJECT}}" => text.Replace("{ {SUBJECT}}", string.Empty),
                    _ => text
                }
                : text.Substring(firstBracket, lastBracket - firstBracket + 1) switch
                {
                    "{ {SUBJECT} }" => text.Replace("{ {SUBJECT} }", $" {subject} "),
                    "{{SUBJECT}}" => text.Replace("{{SUBJECT}}", $"{subject}"),
                    "{{SUBJECT} }" => text.Replace("{{SUBJECT} }", $"{subject} "),
                    "{ {SUBJECT}}" => text.Replace("{ {SUBJECT}}", $" {subject}"),
                    _ => text
                };
        }


        private static string AddSignatureIfNecessary(string text, string? signature)
        {
            return signature is null ? text : $"{text} --{signature}";
        }
    }
}