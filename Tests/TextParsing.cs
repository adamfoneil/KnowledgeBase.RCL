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
            var output = Metadata.ParseKeyValuePair(input);
            Assert.IsTrue(output.Key.Equals(key));
            Assert.IsTrue(output.Value.Equals(value));
        }

        [TestMethod]
        public void ParseSampleContent()
        {
            var content = Static.GetResource("Tests.Resources.Sample.md");

            var output = Metadata.Parse(content, Metadata.TitleMacro);

            Assert.IsTrue(output.Directives.Count == 3);
            Assert.IsTrue(output.Directives.ContainsKey("route"));
            Assert.IsTrue(output.Directives["route"].Equals("whatever\\this\\is\\Umma Gumma"));

            Assert.IsTrue(output.CleanMarkdown.Equals(
@"# Umma Gumma
This is some markdown for the people with a directive up top.

Here is a list of something:

- this
- that
- other

I am a [link](/somewhere) for the people."));
        }
    }
}
