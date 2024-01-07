using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.Text;

namespace BatchEval.Core;

internal class PromptScoreEval : IEvaluator<int>
{
    private Kernel kernel;

    private KernelFunction function;

    public string Id { get; }

    public PromptScoreEval(string id, Kernel kernel, KernelFunction function)
    {
        this.function = function;
        this.kernel = kernel;
        this.Id = id;
    }

    public async Task<int> Eval(ModelOutput modelOutput)
    {
        var promptArgs = new KernelArguments
            {
                { "Question", modelOutput.Input },
                { "Answer", modelOutput.Output }
            };

        var evalResult = await function.InvokeAsync(kernel, promptArgs);

        var evalInt = int.Parse(evalResult.ToString());

        return evalInt;
    }
}