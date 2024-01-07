namespace BatchEval.Core;

internal interface IInputProcessor<T>
{
    public Task<ModelOutput> Process(T userInput);
}