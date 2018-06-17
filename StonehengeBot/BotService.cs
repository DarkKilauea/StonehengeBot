using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StonehengeBot
{
    /// <summary>
    /// Hosts the basic functionality for the bot
    /// </summary>
    [UsedImplicitly]
    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _token;

        /// <summary>
        /// Construct the host for the bot
        /// </summary>
        /// <param name="logger">Logger to use for outputting diagnostic messages</param>
        /// <param name="client">Discord client which will be used for communication to discord's servers</param>
        /// <param name="commandService">Command service for hosting command modules</param>
        /// <param name="serviceProvider">Service provider containing all the services needed by command modules</param>
        /// <param name="token">Bot token for logging into the discord system</param>
        public BotService(DiscordSocketClient client, CommandService commandService, ILogger<BotService> logger,  IServiceProvider serviceProvider, string token)
        {
            _logger = logger;
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _token = token;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
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
