using CommandLine;

namespace MarkdownPublisher.Models
{
    internal class CommandOptions
    {
        [Option("source-path", HelpText = "Location of markdown source files")]
        public string SourcePath { get; set; } = default!;

        [Option("publisher", HelpText = "Name of the publisher engine to use")]
        public string Publisher { get; set; } = default!;
    }
}
