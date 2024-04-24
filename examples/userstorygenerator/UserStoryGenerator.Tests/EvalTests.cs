using BatchEval.Core;
using UserStoryGenerator;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

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
    [InlineData("zoo", "Learn to swim", "duck")]
    [InlineData("IoT project", "IaC for base infrastructure", "developer")]
    [InlineData("IoT new product", "spike in the data processor in the edge", "software engineer")]
    [InlineData("catering", "try beer in different bars", "connoisseur")]
    public async void Should_BeCoherentAndGrounded(string description, string projectContext, string personaName)
    {
        var kernel = _promptEvaluator.Kernel;

        var userStorySkill = UserStorySkill.Create(kernel);

        var userStory = await userStorySkill.GetUserStory(description, projectContext, personaName);

        var modelOutput = new ModelOutput()
        {
            Input = $"Generate a user story for {personaName} so it can {description}",
            Output = $"{userStory!.Title} - {userStory!.Description}"
        };
        
        var coherence = await (new CoherenceEval(kernel)).Eval(modelOutput);
        var groundedness = await (new GroundednessEval(kernel)).Eval(modelOutput);
        var relevance = await (new RelevanceEval(kernel)).Eval(modelOutput);
        
        Assert.Multiple(
            () => Assert.True(coherence >= 3, $"Coherence of {userStory.Title} - score {coherence} [min 3]"),
            () => Assert.True(groundedness >= 3, $"Groundedness of {userStory.Title} - score {groundedness} [min 3]"),
            () => Assert.True(relevance >= 3, $"Relevance of {userStory.Title} - score {relevance} [min 3]")
        );
    }
}