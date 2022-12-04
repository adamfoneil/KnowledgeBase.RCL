using CommandLine;
using MarkdownPublisher.Interfaces;
using MarkdownPublisher.Models;
using System.Text.Json;

var configFile = FindMdPubConfigFile();
var json = await File.ReadAllTextAsync(configFile);
var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? throw new Exception("Error parsing config json");

// might theoretically support different publish targets,
// so they would go in this dictionary
var publishers = new Dictionary<string, IPublisher>()
{
    ["Azure"] = new MarkdownPublisher.Azure.Publisher(config["Azure"])
};

await Parser.Default.ParseArguments<CommandOptions>(args).WithParsedAsync(async options =>
{
    var sourcePath = string.IsNullOrEmpty(options.SourcePath) ? 
        Environment.CurrentDirectory : 
        options.SourcePath;

    var publisher = string.IsNullOrEmpty(options.Publisher) ?
        publishers.First().Value :
        publishers[options.Publisher];

    await publisher.PublishAsync(sourcePath);
});



