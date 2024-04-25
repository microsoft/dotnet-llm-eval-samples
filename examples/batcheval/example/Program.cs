using Microsoft.SemanticKernel;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.SKEval;

namespace BatchEvalSample;

class Program
{
    private static Kernel CreateAndConfigureKernel()
    {
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
            Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
            Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!);

        return builder.Build();
    }

    static async Task Main()
    {
        var kernel = CreateAndConfigureKernel();

        var fileName = "assets/data.jsonl";

        Console.WriteLine($"Reading {fileName}, press a key to continue ...");
        Console.ReadLine();

        var kernelFunctions = kernel.CreatePluginFromPromptDirectory("Prompts");

        var batchEval = new BatchEval<UserInput>();

        batchEval
            .AddEvaluator(new PromptScoreEval("coherence", kernel, kernelFunctions["coherence"]))
            .AddEvaluator(new LenghtEval());

        await batchEval
            .WithInputProcessor(new UserStoryCreator(kernel))
            .WithJsonl(fileName)
            .Run();

        Console.WriteLine($"Complete.");
        Console.ReadLine();
    }
}
