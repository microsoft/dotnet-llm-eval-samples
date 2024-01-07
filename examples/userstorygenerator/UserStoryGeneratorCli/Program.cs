using UserStoryGenerator;
using System.CommandLine.Parsing;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

var serviceCollection = new ServiceCollection();

ConfigureServices(serviceCollection, args);

using var serviceProvider = serviceCollection.BuildServiceProvider();

var rootCommand = new RootCommand();

var personaOption = new Option<string>("--persona")
{
    IsRequired = true,
};

var projectContextOption = new Option<string>("--projectContext")
{
    IsRequired = true,
};

var descriptionOption = new Option<string>("--description")
{
    IsRequired = true,
};

var loggerFactory = serviceProvider.GetService<ILoggerFactory>()!;

var generateUserStory = new Command("generate-user-story", "Generate a user story");

generateUserStory.SetHandler(async (persona, description, project) =>
{
    var userStorySkill = serviceProvider.GetRequiredService<UserStorySkill>();
    var userStory = await userStorySkill.GetUserStory(description, project, persona);

    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(userStory));
}, personaOption, descriptionOption, projectContextOption);

rootCommand.AddCommand(generateUserStory);

var result = await rootCommand.InvokeAsync(args);

return result;

static void ConfigureServices(ServiceCollection serviceCollection, string[] args)
{
    serviceCollection
        .AddLogging(configure =>
        {
            configure.AddSimpleConsole(options => options.TimestampFormat = "hh:mm:ss ");

            if (args.Any("--debug".Contains))
            {
                configure.SetMinimumLevel(LogLevel.Debug);
            }
        })
        .AddSingleton((sp) => 
        {
            var builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(
                 Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL")!,
                 Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
                 Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!);

            return builder.Build();
        })
        .AddSingleton((sp) => 
        {
            return UserStorySkill.Create(sp.GetRequiredService<Kernel>());
        });
}