using UserStoryGenerator;
using BatchEval.Core;
using Microsoft.SemanticKernel;

namespace BatchEval;

internal class UserStoryCreator : IInputProcessor<UserInput>
{
    private readonly UserStorySkill userStoryGenerator;

    public UserStoryCreator(Kernel kernel)
    {
        this.userStoryGenerator = UserStorySkill.Create(kernel);
    }

    public async Task<ModelOutput> Process(UserInput userInput)
    {
        var userStory = await userStoryGenerator.GetUserStory(
            userInput.Description,
            userInput.ProjectContext,
            userInput.Persona);

        return new ModelOutput() {
            Input = $"Generate a user story for {userInput.Persona} so it can {userInput.Description}",
            Output = $"{userStory!.Title} - {userStory!.Description}"
        };
    }
}