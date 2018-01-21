using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JetBrains.Annotations;
using StonehengeBot.Data;

namespace StonehengeBot
{
    /// <summary>
    /// Contains commands for retrieving information about ESO
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly PledgesRepository _pledgesRepository;

        /// <summary>
        /// Construct a module providing command handlers with basic information about ESO
        /// </summary>
        /// <param name="pledgesRepository"></param>
        public InfoModule(PledgesRepository pledgesRepository)
        {
            _pledgesRepository = pledgesRepository;
        }

        /// <summary>
        /// Echo a message for debugging purposes.
        /// </summary>
        /// <param name="message">Text to echo</param>
        [Command("echo")]
        [Summary("Echos a message for debugging purposes.")]
        public async Task EchoAsync([Remainder] [Summary("Text to echo")] string message)
        {
            await ReplyAsync(message);
        }

        /// <summary>
        /// Print out the active undaunted pledge quests for today and tomorrow.
        /// </summary>
        [Command("pledges")]
        [Summary("Print out the active undaunted pledge quests for today and tomorrow.")]
        public async Task GetPledgesAsync()
        {
            var todaysPledges = string.Join('\n',
                _pledgesRepository.GetPledgesForToday().Select(pledge => pledge.DungeonName)
            );

            var tomorrowsPledges = string.Join('\n',
                _pledgesRepository.GetPledgesForTomorrow().Select(pledge => pledge.DungeonName)
            );

            var embed = new EmbedBuilder()
                .WithTitle(_pledgesRepository.GetTitle())
                .WithUrl(_pledgesRepository.GetUrl().AbsoluteUri)
                .WithColor(Color.DarkGreen)
                .WithFooter("stonehenge.bot")
                .WithCurrentTimestamp()
                .AddInlineField("Today's Pledges", todaysPledges)
                .AddInlineField("Tomorrow's Pledges", tomorrowsPledges)
                .Build();

            await ReplyAsync(string.Empty, embed: embed);
        }
    }
}
