namespace BatchEval.Core;

internal interface IEvaluator<T>
{
    public string Id { get; }

    public Task<T> Eval(ModelOutput modelOutput);
}