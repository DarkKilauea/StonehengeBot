using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StonehengeBot
{
    internal static class Program
    {
        private const string EnvironmentPrefix = "STONEHENGEBOT_";

        private static void Main(string[] args)
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(EnvironmentPrefix)
                .AddCommandLine(args)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(configuration);
                    builder.AddConsole();
                })
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<Bot>()
                .BuildServiceProvider(true);

            var bot = serviceProvider.GetRequiredService<Bot>();
            bot.Run().GetAwaiter().GetResult();
        }
    }
}
