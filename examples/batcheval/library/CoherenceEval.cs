using Microsoft.SemanticKernel;

namespace BatchEval.Core;

public class CoherenceEval : PromptScoreEval
{
    public CoherenceEval(Kernel kernel) : base("coherence", kernel, "_prompts.coherence.skprompt.txt") {}
}