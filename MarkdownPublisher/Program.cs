using CommandLine;
using MarkdownPublisher.Interfaces;
using MarkdownPublisher.Models;
using System.Text.Json;

var configFile = FindMdPubConfigFile();
var json = await File.ReadAllTextAsync(configFile);
var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? throw new Exception("Error parsing config json");

// might theoretically support different publish targets,
// so they would go in this dictionary
var publishers = new Dictionary<string, Func<string, IPublisher>>()
{
    ["Azure"] = (sourcePath) => new MarkdownPublisher.Azure.Publisher(sourcePath, config["Azure"])
};

await Parser.Default.ParseArguments<CommandOptions>(args).WithParsedAsync(async options =>
{
    var sourcePath = string.IsNullOrEmpty(options.SourcePath) ? 
        Environment.CurrentDirectory : 
        options.SourcePath;

    var publisherAccessor = string.IsNullOrEmpty(options.Publisher) ?
        publishers.First().Value :
        publishers[options.Publisher];

    // this allows project-level settings in a KB to be injected into the configuration,
    // which depends on knowing the project source path
    var publisher = publisherAccessor.Invoke(sourcePath);

    await publisher.PublishAsync(sourcePath);
});



