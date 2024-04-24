using Microsoft.SemanticKernel;

namespace BatchEval.Core;

public class GroundednessEval : PromptScoreEval
{
    public GroundednessEval(Kernel kernel) : base("groundedness", kernel, "_prompts.groundedness.skprompt.txt") {}
}