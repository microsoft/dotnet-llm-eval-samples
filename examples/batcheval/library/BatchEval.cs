using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.Text;

namespace BatchEval.Core;

public class BatchEval<T>
{
    IList<IEvaluator<int>> intEvaluators = new List<IEvaluator<int>>();

    IList<IEvaluator<bool>> boolEvaluators = new List<IEvaluator<bool>>();

    string? fileName;

    IInputProcessor<T>? inputProcessor;

    public string? OtlpEndpoint { get; set; } = default!;

    public BatchEval<T> WithInputProcessor(IInputProcessor<T> inputProcessor)
    {
        this.inputProcessor = inputProcessor;
        return this;
    }

    public BatchEval<T> AddEvaluator(IEvaluator<int> evaluator)
    {
        intEvaluators.Add(evaluator);
        return this;
    }

    public BatchEval<T> AddEvaluator(IEvaluator<bool> evaluator)
    {
        boolEvaluators.Add(evaluator);
        return this;
    }

    public async Task Run()
    {
        await ProcessUserInputFile();
    }

    public BatchEval<T> WithJsonl(string fileName)
    {
        this.fileName = fileName;
        return this;
    }

    private async Task ProcessUserInputFile()
    {
        var meterId = "llm-eval";
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

        var boolCounters = new Dictionary<string, Counter<int>>();

        foreach (var evaluator in intEvaluators)
        {
            var histogram = meter.CreateHistogram<int>($"{evaluator.Id.ToLowerInvariant()}.score");
            histograms.Add(evaluator.Id, histogram);
        }

        foreach (var evaluator in boolEvaluators)
        {
            boolCounters.Add(
                $"{evaluator.Id.ToLowerInvariant()}.failure", 
                meter.CreateCounter<int>($"{evaluator.Id.ToLowerInvariant()}.failure"));

            boolCounters.Add(
                $"{evaluator.Id.ToLowerInvariant()}.success", 
                meter.CreateCounter<int>($"{evaluator.Id.ToLowerInvariant()}.success"));
        }

        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            var userInput = System.Text.Json.JsonSerializer.Deserialize<T>(line);

            var modelOutput = await inputProcessor!.Process(userInput!);

            Console.WriteLine($"Q: {modelOutput.Input}");
            Console.WriteLine($"A: {modelOutput.Output}");

            counter.Add(1);

            foreach (var evaluator in intEvaluators)
            {
                var score = await evaluator.Eval(modelOutput);

                Console.WriteLine($"E: {evaluator.Id.ToLowerInvariant()} S: {score}");
                histograms[evaluator.Id.ToLowerInvariant()].Record(score);
            }

            foreach (var evaluator in boolEvaluators)
            {
                var evalResult = await evaluator.Eval(modelOutput);

                Console.WriteLine($"E: {evaluator.Id.ToLowerInvariant()} R: {evalResult}");

                if (evalResult) {
                    boolCounters[$"{evaluator.Id.ToLowerInvariant()}.success"].Add(1);
                } else {
                    boolCounters[$"{evaluator.Id.ToLowerInvariant()}.failure"].Add(1);
                }
            }
        }
    }

    private Meter CreateMeter(string meterId)
    {
        var builder = Sdk.CreateMeterProviderBuilder()
            .AddMeter(meterId);

        foreach (var evaluator in intEvaluators)
        {
            builder.AddView(
                instrumentName: $"{evaluator.Id.ToLowerInvariant()}.score",
                new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 1, 2, 3, 4, 5 } });
        }

        if (string.IsNullOrEmpty(OtlpEndpoint))
        {
            builder.AddConsoleExporter();
        } else {
            builder.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(OtlpEndpoint);
            });
        }

        builder.AddMeter("Microsoft.SemanticKernel*");
    
        builder.Build();

        return new Meter(meterId);
    }
}