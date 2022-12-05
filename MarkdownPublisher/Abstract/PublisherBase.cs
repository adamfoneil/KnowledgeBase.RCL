using FolderStructure;
using Knowledgebase.Shared;
using Markdig;
using MarkdownPublisher.Extensions;
using MarkdownPublisher.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MarkdownPublisher.Abstract
{
    internal abstract class PublisherBase<TConfig> : IPublisher where TConfig : IProjectOptions
    {        
        private readonly TConfig _configObj;

        protected PublisherBase(string sourcePath, Dictionary<string, string> config)
        {
            AddProjectOptions(sourcePath, config);
            _configObj = BuildConfiguration(config);
        }

        protected TConfig Config => _configObj;        

        protected abstract TConfig BuildConfiguration(Dictionary<string, string> config);

        protected abstract Task PublishFileAsync(string sourceFile, string targetFile);

        public async Task PublishAsync(string sourcePath)
        {
            var sourceFiles = Directory.GetFiles(sourcePath, "*.md", SearchOption.AllDirectories);

            var routes = new HashSet<NavEntry>();

            await foreach (var file in BuildHtmlFiles(sourcePath, sourceFiles))
            {
                try
                {
                    if (file.Directives.TryGetValue("route", out string? val) && !string.IsNullOrEmpty(val))
                    {
                        routes.Add(new NavEntry() { Route = val, HostHref = file.Href, Href = file.RemoteFilename });
                    }

                    await PublishFileAsync(file.TempFilename, file.RemoteFilename);
                }
                finally
                {
                    File.Delete(file.TempFilename);
                }
            }
            
            var nav = Folder<NavEntry>.From(routes, r => r.Route);
            var navJson = JsonSerializer.Serialize(nav);
            const string navFilename = "nav.json";
            var navFile = Path.Combine(sourcePath, navFilename);
            await File.WriteAllTextAsync(navFile, navJson);
            await PublishFileAsync(navFile, Config.BlobPrefix + navFilename);
        }

        private void AddProjectOptions(string sourcePath, Dictionary<string, string> config)
        {
            var configFile = Path.Combine(sourcePath, "config.json");
            if (File.Exists(configFile))
            {
                var json = File.ReadAllText(configFile);
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? throw new Exception($"Error reading {configFile} content.");
                foreach (var kp in dictionary) config.Add(kp.Key, kp.Value);
            }
        }

        private async IAsyncEnumerable<FileInfo> BuildHtmlFiles(string sourcePath, string[] sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                var markdown = await File.ReadAllTextAsync(sourceFile);
                var (directives, cleanMarkdown, title) = Metadata.Parse(markdown, Metadata.TitleMacro);

                UpdateLinks(ref cleanMarkdown);

                var html = Markdown.ToHtml(cleanMarkdown);

                var tempHtmlFile = await SaveTempHtmlFileAsync(sourceFile, html);
                var baseFilename = tempHtmlFile.Substring(sourcePath.Length + 1);
                var targetName = Config.BlobPrefix + baseFilename;
                var href = Config.HostRoutePrefix + baseFilename;

                yield return new FileInfo()
                {
                    TempFilename = tempHtmlFile,
                    RemoteFilename = targetName,
                    Directives = directives,
                    Title = title,
                    Href = href
                };
            }
        }

        /// <summary>
        /// applies the HostRoutePrefix to all links in markdown content,
        /// and replaces .md suffixes with .html
        /// </summary>
        private void UpdateLinks(ref string markdown)
        {
            markdown = Regex.Replace(markdown, @"\[(.*?)\](?<target>\(.*?\))", BuildLink);            

            string BuildLink(Match match)
            {
                var result = $"[{match.Groups[1].Value}]";
                var target = Metadata.RemoveStartEnd(match.Groups[2].Value, "(", ")");
                result += $"({Config.HostRoutePrefix}{target.Replace(".md", ".html")})";
                return result;
            }
        }        

        private async Task<string> SaveTempHtmlFileAsync(string file, string content)
        {            
            var outputPath = Path.GetDirectoryName(file) ?? throw new Exception($"Couldn't get directory name of {file}");

            var outputFile = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file) + ".html");

            await File.WriteAllTextAsync(outputFile, content);

            return outputFile;
        }

        private class FileInfo
        {
            public Dictionary<string, string> Directives { get; init; } = new();
            public string TempFilename { get; init; } = default!;
            public string RemoteFilename { get; init; } = default!;
            public string Title { get; init; } = default!;
            public string Href { get; init; } = default!;
        }
    }
}
