using System.Reflection;

internal partial class Program
{
    internal static IEnumerable<string> GetMarkdownFiles()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Couldn't determine base path");
        return Directory.GetFiles(path, "*.md", SearchOption.AllDirectories);
    }
}
