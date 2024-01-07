using Microsoft.SemanticKernel;
using System.Text.Json;

namespace UserStoryGenerator;

public class UserStorySkill
{
    private readonly KernelFunction _createUserStoryFunction;

    private readonly Kernel _kernel;

    public static UserStorySkill Create(Kernel kernel)
    {
        //string promptConfigString = EmbeddedResource.Read("_prompts.userstoryclassic.config.json")!;
        //var promptConfig = PromptTemplateConfig.FromJson(promptConfigString);

        string promptTemplate = EmbeddedResource.Read("_prompts.userstoryclassic.skprompt.txt")!;

        return new UserStorySkill(kernel, kernel.CreateFunctionFromPrompt(promptTemplate));
    }

    public UserStorySkill(Kernel kernel, KernelFunction promptFunction)
    {
        _createUserStoryFunction = promptFunction;
        _kernel = kernel;
    }

    public async Task<UserStory?> GetUserStory(string description, string? projectContext = null, string? personaName = null)
    {
        var context = new KernelArguments
        {
            { "ProjectContext", projectContext ?? "software development project" },
            { "ContextTopic", description! },
            { "Persona", personaName ?? "software engineer" }
        };

        var result = await _createUserStoryFunction.InvokeAsync(_kernel, context);

        if (!result.ToString().IsValidJson())
        {
            throw new Exception("Invalid prompt result, not valid json");
        }

        var userStory = JsonSerializer.Deserialize<UserStory>(result.ToString(), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return userStory;
    }
}