namespace BatchEval.Core;

public interface IInputProcessor<T>
{
    public Task<ModelOutput> Process(T userInput);
}