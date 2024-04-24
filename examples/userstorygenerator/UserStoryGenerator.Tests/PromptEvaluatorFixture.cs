using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace UserStoryGenerator.Tests;

public class PromptEvalDotNetFixture
{
    public Kernel Kernel { get; }

    public PromptEvalDotNetFixture()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var config = configurationBuilder.Build();

        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
                config["AZURE_OPENAI_MODEL"]!,
                config["AZURE_OPENAI_ENDPOINT"]!,
                config["AZURE_OPENAI_KEY"]!);

        Kernel = builder.Build();
    }
}
