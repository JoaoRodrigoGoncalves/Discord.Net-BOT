using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Ping: ~" + Context.Client.Latency + "ms");
        }
    }
}
