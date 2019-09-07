using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Discord;

namespace LeBot.Modules
{
    public class Fe : ModuleBase<SocketCommandContext>
    {
        [Command("fe")]
        [Alias("coins", "money", "coin")]
        public async Task FeAsync([Remainder] string user = null)
        {
            string userid = null;
            string String = null;
            bool self = true;
            if(user == null)
            {
                userid = "" + Context.Message.Author.Id;
                String = Context.Message.Author + ", tu tens";
                self = true;
            }
            else
            {
                var charToRemove = new string[] { "<", "@", "!", ">" };
                foreach (var c in charToRemove)
                {
                    user = user.Replace(c, string.Empty);
                }
                userid = user;
                String = Context.Client.GetUser(ulong.Parse(user)) +" tem";
                self = false;
            }
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            var getColors = new Functions.RandomBuilderColor();

            try
            {
                connection.Open();
                command.CommandText = "SELECT coins FROM currency WHERE userid='" + userid + "' LIMIT 1";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();
                EmbedBuilder coinsAmount = new EmbedBuilder();
                if (coinsReader.Read()) {
                    uint userCoins = coinsReader.GetUInt32("coins");
                    coinsAmount.WithTitle(String + " " + coinsReader.GetInt32("coins") + " de fé!")
                        .WithColor(getColors.MessageBuilderColor());
                    if (self)
                    {
                        coinsAmount.WithThumbnailUrl("https://cdn.discordapp.com/avatars/" + Context.Message.Author.Id + "/" + Context.Message.Author.AvatarId + ".png?size=128");
                    }
                    else
                    {
                        coinsAmount.WithThumbnailUrl("https://cdn.discordapp.com/avatars/" + Context.Client.GetUser(ulong.Parse(userid)).Id + "/" + Context.Client.GetUser(ulong.Parse(userid)).AvatarId + ".png?size=128");
                    }
                    coinsReader.Close();
                }
                else
                {
                    coinsReader.Close();
                    command.CommandText = "INSERT INTO currency (userid) VALUES ('" + Context.Message.Author.Id + "') LIMIT 1";
                    command.ExecuteNonQuery();

                    coinsAmount.WithTitle(String + " 10 de fé!")
                        .WithColor(getColors.MessageBuilderColor());
                    if (self)
                    {
                        coinsAmount.WithThumbnailUrl("https://cdn.discordapp.com/avatars/" + Context.Message.Author.Id + "/" + Context.Message.Author.AvatarId + ".png?size=128");
                    }
                    else
                    {
                        coinsAmount.WithThumbnailUrl("https://cdn.discordapp.com/avatars/" + Context.Client.GetUser(ulong.Parse(userid)).Id + "/" + Context.Client.GetUser(ulong.Parse(userid)).AvatarId + ".png?size=128");
                    } 
                }
                coinsAmount.WithDescription("Consegue mais fé com comandos como \"\\missa\"!");
                await ReplyAsync("", false, coinsAmount);
                connection.Close();
            }
            catch(MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }
    }
}