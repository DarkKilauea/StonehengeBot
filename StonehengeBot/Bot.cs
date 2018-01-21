using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StonehengeBot
{
    /// <summary>
    /// Hosts the basic functionality for the bot
    /// </summary>
    [UsedImplicitly]
    public class Bot
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<Bot> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Construct the host for the bot
        /// </summary>
        /// <param name="configuration">Configuration for the bot</param>
        /// <param name="logger">Logger to use for outputting diagnostic messages</param>
        /// <param name="client">Discord client which will be used for communication to discord's servers</param>
        /// <param name="commandService">Command service for hosting command modules</param>
        /// <param name="serviceProvider">Service provider containing all the services needed by command modules</param>
        public Bot(IConfigurationRoot configuration, ILogger<Bot> logger, DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Run the bot, blocking until the program is exited
        /// </summary>
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
            var result = await _commandService.ExecuteAsync(context, argPosition, _serviceProvider);
            if (!result.IsSuccess)
            {
                _logger.LogError("{ErrorReason}: {Message} in {Channel} by {Author}", result.ErrorReason, userMessage.Content, userMessage.Channel, userMessage.Author);
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
