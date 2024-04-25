using Microsoft.SemanticKernel;

namespace BatchEval.Core;

public class ModelOutput
{
    public string Input { get; set; } = default!;

    public string Output { get; set; } = default!;
}