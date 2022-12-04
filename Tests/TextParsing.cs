using MarkdownPublisher.Extensions;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class TextParsing
    {
        [TestMethod]
        [DataRow("@route \"whatever\\this\\is\"", "route", @"whatever\this\is")]
        [DataRow("@route whatever\\this\\is", "route", @"whatever\this\is")]
        public void ParseDirectives(string input, string key, string value)
        {
            var output = Directives.ParseKeyValuePair(input);
            Assert.IsTrue(output.Key.Equals(key));
            Assert.IsTrue(output.Value.Equals(value));
        }

        [TestMethod]
        public void ParseSampleContent()
        {
            var content = GetResource("Tests.Resources.Sample.md");

            var output = Directives.Parse(content);

            Assert.IsTrue(output.Directives.Count == 3);
            Assert.IsTrue(output.Directives.ContainsKey("route"));

            Assert.IsTrue(output.CleanMarkdown.Equals(
@"# Umma Gumma
This is some markdown for the people with a directive up top.

Here is a list of something:

- this
- that
- other

I am a [link](/somewhere) for the people."));
        }

        private static string GetResource(string resourceName)
        {
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ?? throw new Exception($"Resource {resourceName} not found");
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
