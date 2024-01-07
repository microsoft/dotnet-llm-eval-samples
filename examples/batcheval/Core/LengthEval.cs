namespace BatchEval.Core;

internal class LenghtEval : IEvaluator<int>
{
    public string Id { get; } = "length";

    public async Task<int> Eval(ModelOutput modelOutput)
    {
        return modelOutput.Output.Length;
    }
}