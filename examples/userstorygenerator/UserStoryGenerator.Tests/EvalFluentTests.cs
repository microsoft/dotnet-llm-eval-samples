using BatchEval.Core.Tests;
using UserStoryGenerator;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace UserStoryGenerator.Tests;

public class EvalFluentTests : IClassFixture<PromptEvalDotNetFixture>
{
    private PromptEvalDotNetFixture _promptEvaluator = default!;

    private readonly ITestOutputHelper _output;

    public EvalFluentTests(PromptEvalDotNetFixture fixture, ITestOutputHelper output)
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
        
        modelOutput.With(kernel).ShouldBeCoherent();
        modelOutput.With(kernel).ShouldBeGrounded();
        modelOutput.With(kernel).ShouldBeRelevant();
    }
}