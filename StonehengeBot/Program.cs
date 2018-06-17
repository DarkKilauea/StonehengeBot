using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddJsonFile("appsettings.json");
                    configurationBuilder.AddEnvironmentVariables(EnvironmentPrefix);
                    configurationBuilder.AddCommandLine(args);
                })
                .ConfigureServices((context, collection) =>
                {
                    collection.AddOptions();
                    collection.Configure<PledgeOptions>(context.Configuration.GetSection("Pledges"));

                    collection.AddSingleton<PledgesRepository>();
                    collection.AddSingleton<DiscordSocketClient>();
                    collection.AddSingleton<CommandService>();
                    collection.AddSingleton<IHostedService, BotService>(provider => new BotService(
                            provider.GetRequiredService<DiscordSocketClient>(),
                            provider.GetRequiredService<CommandService>(),
                            provider.GetRequiredService<ILogger<BotService>>(),
                            provider,
                            context.Configuration["Token"]
                        )
                    );
                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                });

            builder.RunConsoleAsync().GetAwaiter().GetResult();
        }
    }
}
