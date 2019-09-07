using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task HelpAsync([Remainder] string sub = null)
        {
            if (string.IsNullOrEmpty(sub))
            {
                await defaultHelp();
            }
            else
            {
                await subHub(sub);
            }
        }

        public async Task subHub([Remainder] string command)
        {
            switch (command)
            {
                case "shop":
                    await shopHelp();
                    break;
                default:
                    await defaultHelp();
                    break;
            }
        }

        public async Task shopHelp()
        {
            var getColor = new Functions.RandomBuilderColor();
            EmbedBuilder helpCommands = new EmbedBuilder();

            helpCommands.WithTitle("Comandos da shop d'**padre**!")
                .WithDescription("Comandos da *shop*")
                .WithColor(getColor.MessageBuilderColor())
                .AddInlineField("\\shop `{ordem [id/price/name]}` `{sentido [ASC/DESC]}`", "Mostra toda a shop")
                .AddInlineField("\\buy `{id}`","Comprar o item selecionado em `id`");

            await ReplyAsync("", false, helpCommands);
        }

        public async Task defaultHelp()
        {
            var getColor = new Functions.RandomBuilderColor();
            EmbedBuilder helpCommands = new EmbedBuilder();

            helpCommands.WithTitle("Comandos d'**O Padre**!")
                .WithDescription("Todos os comandos d'**O Padre**!")
                .WithColor(getColor.MessageBuilderColor())
                .AddInlineField("\\help", "Este menu de ajuda :expressionless:")
                .AddInlineField("\\help shop", "Menu de ajuda da \\shop")
                .AddInlineField("\\credits", "Autores d'**O Padre**")
                .AddInlineField("\\dab", "Precisas de um update :neutral_face:")
                .AddInlineField("\\shutup", "**O Padre** vai fingir que não te está a ouvir...")
                .AddInlineField("\\fe", "Mostra a tua \"fé\"!")
                .AddInlineField("\\lapada `{gajo}`", "Espeta um lapada nesse gajo")
                .AddInlineField("\\give `{quantia}` `{gajo}`", "Dar \"fé\" a um gajo :clap: :pray:")
                .AddInlineField("\\festa", "Não é uma festa sem foguetes! :fireworks:")
                .AddInlineField("\\missa", "Vai á missa e pode ser que o padre te dê muita fé :pray:")
                .AddInlineField("\\top", "Os mais ligados a Deus de sempre! :raised_hands:")
                .AddInlineField("\\bet {quantidade}", "Duplica, triplica ou perde toda a fé que apostares!");

            await ReplyAsync("", false, helpCommands.Build());
        }
    }
}
