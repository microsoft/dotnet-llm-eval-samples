using Microsoft.SKEval;
using Xunit.Abstractions;

namespace UserStoryGenerator.Tests;

public class EvalTests : IClassFixture<PromptEvalDotNetFixture>
{
    private PromptEvalDotNetFixture _promptEvaluator = default!;

    private readonly ITestOutputHelper _output;

    public EvalTests(PromptEvalDotNetFixture fixture, ITestOutputHelper output)
    {
        _promptEvaluator = fixture;
        _output = output;
    }

    [Theory]
    [InlineData("CRM software", "Create a login page", "marketing")]
    [InlineData("IoT project", "IaC for base infrastructure", "developer")]
    [InlineData("IoT new product", "spike in the data processor in the edge", "software engineer")]
    public async void Should_BeCoherentAndGrounded(string projectContext, string description, string personaName)
    {
        var kernel = _promptEvaluator.Kernel;

        var userStorySkill = UserStorySkill.Create(kernel);

        var userStory = await userStorySkill.GetUserStory(description, projectContext, personaName);

        var modelOutput = new ModelOutput()
        {
            Input = $"In a {projectContext}, generate a user story for a {personaName} persona, doing {description}",
            Output = $"{userStory!.Title} - {userStory!.Description}"
        };

        _output.WriteLine($"Q: {modelOutput.Input}");
        _output.WriteLine($"A: {modelOutput.Output}.");
        
        var coherence = await new CoherenceEval(kernel).Eval(modelOutput);
        var groundedness = await new GroundednessEval(kernel).Eval(modelOutput);
        var relevance = await new RelevanceEval(kernel).Eval(modelOutput);

        Assert.Multiple(
            () => Assert.True(coherence >= 3, $"Coherence of {userStory.Title} - score {coherence}, expecting min 3."),
            () => Assert.True(groundedness >= 3, $"Groundedness of {userStory.Title} - score {groundedness}, expecting min 3."),
            () => Assert.True(relevance >= 2, $"Relevance of {userStory.Title} - score {relevance}, expecting min 2.")
        );
    }
}