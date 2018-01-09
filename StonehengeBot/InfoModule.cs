using System.Threading.Tasks;
using Discord.Commands;

namespace StonehengeBot
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echos a message for debugging purposes.")]
        public async Task EchoAsync([Remainder] [Summary("Text to echo")] string message)
        {
            await ReplyAsync(message);
        }
    }
}
