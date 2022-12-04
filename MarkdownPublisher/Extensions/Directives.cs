using System.Text.RegularExpressions;

namespace MarkdownPublisher.Extensions
{
    public static class Directives
    {
        public static (string Key, string Value) ParseKeyValuePair(string input) 
        {
            var quoted = Regex.Match(input, @"(\""[^\""]*\"")");
            if (quoted.Success)
            {
                var value = RemoveQuotes(quoted.Value);
                var key = RemoveAtSign(input.Replace(quoted.Value, string.Empty).Trim());
                return (key, value);
            }

            int firstSpace = input.IndexOf(' ');
            if (firstSpace > -1)
            {
                var key = RemoveAtSign(input.Substring(0, firstSpace).Trim());
                var value = input.Substring(firstSpace + 1).Trim();
                return (key, value);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// any initial lines of text starting with "@" are assumed to be key + value pairs
        /// </summary>
        public static (Dictionary<string, string> Directives, string CleanMarkdown) Parse(string content)
        {
            var lines = content.Split("\r\n");

            List<string> directiveLines = new();
            int lineIndex = 0;

            foreach (var line in lines)
            {
                lineIndex++;
                if (line.StartsWith("@"))
                {
                    var addLine = line.Substring(1).Trim();
                    if (addLine.Length > 0) directiveLines.Add(addLine);
                }
                else
                {
                    break;
                }
            }

            var directives = new Dictionary<string, string>();
            foreach (var directive in directiveLines) AddKeyValuePair(directive, directives);

            var cleanMarkdown = string.Join("\r\n", lines[lineIndex..]);

            return (directives, cleanMarkdown);
        }

        private static string RemoveQuotes(string value)
        {
            var result = value;
            if (result.StartsWith("\"")) result = result.Substring(1);
            if (result.EndsWith("\"")) result = result.Substring(0, result.Length - 1);
            return result;
        }

        private static string RemoveAtSign(string value)
        {
            var result = value;
            if (result.StartsWith("@")) result = result.Substring(1);
            return result;
        }
        
        public static void AddKeyValuePair(string input, Dictionary<string, string> dictionary) 
        {
            var keyPair = ParseKeyValuePair(input);
            dictionary.Add(keyPair.Key, keyPair.Value);
        }
    }
}
