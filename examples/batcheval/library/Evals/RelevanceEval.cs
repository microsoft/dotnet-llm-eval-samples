using Microsoft.SemanticKernel;

namespace BatchEval.Core;

public class RelevanceEval : PromptScoreEval
{
    public RelevanceEval(Kernel kernel) : base("relevance", kernel, "_prompts.relevance.skprompt.txt") {}
}