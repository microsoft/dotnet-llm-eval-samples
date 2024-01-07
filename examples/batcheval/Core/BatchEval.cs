using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.Text;

namespace BatchEval.Core;

internal class BatchEval<T>
{
    IList<IEvaluator<int>> evaluators = new List<IEvaluator<int>>();

    string? fileName;

    IInputProcessor<T>? inputProcessor;

    internal BatchEval<T> WithInputProcessor(IInputProcessor<T> inputProcessor)
    {
        this.inputProcessor = inputProcessor;
        return this;
    }

    internal BatchEval<T> AddEvaluator(IEvaluator<int> evaluator)
    {
        evaluators.Add(evaluator);
        return this;
    }

    internal async Task Run()
    {
        await ProcessUserInputFile();
    }

    internal BatchEval<T> WithJsonl(string fileName)
    {
        this.fileName = fileName;
        return this;
    }

    private async Task ProcessUserInputFile()
    {
        var meterId = "AzDoCopilotTests";
        var meter = CreateMeter(meterId);

        const int BufferSize = 128;
        using (var fileStream = File.OpenRead(fileName!))
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
        {
            await ProcessFileLines(streamReader, meter);
        }
    }

    private async Task ProcessFileLines(
       StreamReader streamReader,
       Meter meter)
    {
        var counter = meter.CreateCounter<int>($"prompt.counter");

        var histograms = new Dictionary<string, Histogram<int>>();

        foreach (var evaluator in evaluators)
        {
            var histogram = meter.CreateHistogram<int>($"{evaluator.Id.ToLowerInvariant()}.score");
            histograms.Add(evaluator.Id, histogram);
        }

        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            var userInput = System.Text.Json.JsonSerializer.Deserialize<T>(line);

            var modelOutput = await inputProcessor!.Process(userInput!);

            Console.WriteLine($"Q: {modelOutput.Input}");
            Console.WriteLine($"A: {modelOutput.Output}");

            counter.Add(1);

            foreach (var evaluator in evaluators)
            {
                var score = await evaluator.Eval(modelOutput);

                Console.WriteLine($"E: {evaluator.Id.ToLowerInvariant()} S: {score}");
                histograms[evaluator.Id.ToLowerInvariant()].Record(score);
            }
        }
    }

    private Meter CreateMeter(string meterId)
    {
        var builder = Sdk.CreateMeterProviderBuilder()
            .AddMeter(meterId);

        foreach (var evaluator in evaluators)
        {
            builder.AddView(
                instrumentName: "coherence.score",
                new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 1, 2, 3, 4, 5 } });
        }

        builder.AddConsoleExporter()
               .Build();

        return new Meter(meterId);
    }
}