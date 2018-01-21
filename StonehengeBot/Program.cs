using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StonehengeBot.Configuration;
using StonehengeBot.Data;

namespace StonehengeBot
{
    internal static class Program
    {
        private const string EnvironmentPrefix = "STONEHENGEBOT_";

        private static void Main(string[] args)
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(EnvironmentPrefix)
                .AddCommandLine(args)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddOptions()
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(configuration);
                    builder.AddConsole();
                })
                .Configure<PledgeOptions>(configuration.GetSection("Pledges"))
                .AddSingleton<PledgesRepository>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<Bot>()
                .BuildServiceProvider(true);

            var bot = serviceProvider.GetRequiredService<Bot>();
            bot.Run().GetAwaiter().GetResult();
        }
    }
}
