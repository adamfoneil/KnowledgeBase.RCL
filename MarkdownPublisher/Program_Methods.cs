using System.Reflection;

internal partial class Program
{
    internal static string FindMdPubConfigFile()
    {
        const string fileName = "mdpub.json";

        var folder = Environment.CurrentDirectory;
        var result = string.Empty;

        do
        {
            result = Path.Combine(folder, fileName);
            folder = Directory.GetParent(folder)?.FullName ?? throw new Exception($"No parent directory of {folder}");
        } while (!File.Exists(result));

        return result;
    }
}
