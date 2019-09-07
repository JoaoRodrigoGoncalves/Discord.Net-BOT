using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Credits : ModuleBase<SocketCommandContext>
    {
        [Command("credits")]
        public async Task CreditsAsync()
        {
            var getColor = new Functions.RandomBuilderColor();
            EmbedBuilder creditsBuilder = new EmbedBuilder();
            creditsBuilder.WithTitle("Creditos d'**O Padre**")
                .WithColor(getColor.MessageBuilderColor())
                .AddField("Ideias por:", "@João Rodrigo Gonçalves#9391 \n@Hikaru#0448")
                .AddField("Programação por:", "@João Rodrigo Gonçalves#9391")
                .AddField("Atenção!", "BOT feito em C# e PHP com :heart: para uso privado!");
            await ReplyAsync("", false, creditsBuilder.Build());
        }
    }
}
