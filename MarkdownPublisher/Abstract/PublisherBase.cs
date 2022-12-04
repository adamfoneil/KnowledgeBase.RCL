using Markdig;

namespace MarkdownPublisher.Abstract
{
    internal abstract class PublisherBase<TConfig>
    {
        private readonly TConfig _config;

        protected PublisherBase(string jsonConfig)
        {
            _config = BuildConfiguration(jsonConfig);
        }

        protected abstract TConfig BuildConfiguration(string json);

        protected TConfig Config => _config;

        protected abstract Task PublishFileAsync(string localFile);

        public async Task PublishAsync(string sourcePath, string outputPath)
        {
            var sourceFiles = Directory.GetFiles(sourcePath, "*.md", SearchOption.AllDirectories);
            
            await foreach (var htmlFile in BuildHtmlFiles(sourceFiles, outputPath))
            {
                try
                {
                    await PublishFileAsync(htmlFile);
                }
                finally
                {
                    File.Delete(htmlFile);
                }
            }
        }

        private async IAsyncEnumerable<string> BuildHtmlFiles(string[] sourceFiles, string outputPath)
        {
            foreach (var sourceFile in sourceFiles)
            {
                var markdown = await File.ReadAllTextAsync(sourceFile);
                var html = Markdown.ToHtml(markdown);
                yield return await SaveTempHtmlFileAsync(sourceFile, html);                
            }
        }

        private async Task<string> SaveTempHtmlFileAsync(string file, string content)
        {
            var outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Markdown Publisher");
            
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            var outputFile =  Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file) + ".html");

            await File.WriteAllTextAsync(outputFile, content);

            return outputFile;
        }
    }
}
