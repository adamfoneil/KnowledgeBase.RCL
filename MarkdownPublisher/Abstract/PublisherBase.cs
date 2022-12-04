using Markdig;
using MarkdownPublisher.Extensions;
using MarkdownPublisher.Interfaces;
using Microsoft.Extensions.Azure;
using System.Net.Mail;

namespace MarkdownPublisher.Abstract
{
    internal abstract class PublisherBase<TConfig> : IPublisher
    {
        private readonly TConfig _config;

        protected PublisherBase(Dictionary<string, string> config)
        {
            _config = BuildConfiguration(config);
        }

        protected abstract TConfig BuildConfiguration(Dictionary<string, string> config);

        protected TConfig Config => _config;

        protected abstract Task PublishFileAsync(string sourceFile, string targetFile);

        public async Task PublishAsync(string sourcePath)
        {
            var sourceFiles = Directory.GetFiles(sourcePath, "*.md", SearchOption.AllDirectories);

            var routes = new HashSet<string>();

            await foreach (var file in BuildHtmlFiles(sourcePath, sourceFiles))
            {
                try
                {
                    if (file.Directives.TryGetValue("route", out string? val) && !string.IsNullOrEmpty(val)) routes.Add(val);

                    await PublishFileAsync(file.LocalHtmlFile, file.BlobName);
                }
                finally
                {
                    File.Delete(file.LocalHtmlFile);
                }
            }
        }

        private async IAsyncEnumerable<(Dictionary<string, string> Directives, string LocalHtmlFile, string BlobName)> BuildHtmlFiles(string sourcePath, string[] sourceFiles)
        {
            foreach (var sourceFile in sourceFiles)
            {
                var markdown = await File.ReadAllTextAsync(sourceFile);
                var (directives, cleanMarkdown) = Directives.Parse(markdown, Directives.TitleMacro);                

                var html = Markdown.ToHtml(cleanMarkdown);

                var sourceName = await SaveTempHtmlFileAsync(sourceFile, html);
                var targetName = sourceName.Substring(sourcePath.Length + 1);
                yield return (directives, sourceName, targetName);
            }
        }

        private async Task<string> SaveTempHtmlFileAsync(string file, string content)
        {            
            var outputPath = Path.GetDirectoryName(file) ?? throw new Exception($"Couldn't get directory name of {file}");

            var outputFile = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file) + ".html");

            await File.WriteAllTextAsync(outputFile, content);

            return outputFile;
        }
    }
}
