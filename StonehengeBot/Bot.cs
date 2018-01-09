using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StonehengeBot
{
    public class Bot
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<Bot> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public Bot(IConfigurationRoot configuration, ILogger<Bot> logger, DiscordSocketClient client, CommandService commandService)
        {
            _configuration = configuration;
            _logger = logger;
            _client = client;
            _commandService = commandService;
        }

        public async Task Run()
        {
            _client.Log += LogAsync;

            await InstallCommandsAsync(_client);

            await _client.LoginAsync(TokenType.Bot, _configuration["Token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task InstallCommandsAsync(DiscordSocketClient client)
        {
            client.MessageReceived += MessageReceivedAsync;

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage))
                return;

            var argPosition = 0;
            if (!userMessage.HasCharPrefix('!', ref argPosition) && !userMessage.HasMentionPrefix(_client.CurrentUser, ref argPosition))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _commandService.ExecuteAsync(context, argPosition);
            if (!result.IsSuccess)
            {
                _logger.LogError(result.ErrorReason);
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private Task LogAsync(LogMessage message)
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
