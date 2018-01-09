using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StonehengeBot
{
    public class Bot
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<Bot> _logger;

        public Bot(IConfigurationRoot configuration, ILogger<Bot> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Run()
        {
            using (var client = new DiscordSocketClient())
            {
                client.Log += Log;
                client.MessageReceived += MessageReceived;

                await client.LoginAsync(TokenType.Bot, _configuration["Token"]);
                await client.StartAsync();

                await Task.Delay(-1);
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }
        }

        private Task Log(LogMessage message)
        {
            LogLevel logLevel;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    logLevel = LogLevel.Critical;
                    break;
                case LogSeverity.Error:
                    logLevel = LogLevel.Error;
                    break;
                case LogSeverity.Warning:
                    logLevel = LogLevel.Warning;
                    break;
                case LogSeverity.Info:
                    logLevel = LogLevel.Information;
                    break;
                case LogSeverity.Verbose:
                    logLevel = LogLevel.Trace;
                    break;
                case LogSeverity.Debug:
                    logLevel = LogLevel.Debug;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _logger.Log(logLevel, 0, message, message.Exception, (logMessage, exception) => logMessage.ToString(null, prependTimestamp: false));

            return Task.CompletedTask;
        }
    }
}
