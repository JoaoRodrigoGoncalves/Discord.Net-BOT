using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;

namespace LeBot.Modules
{
    public class Top : ModuleBase<SocketCommandContext>
    {
        [Command("top")]
        public async Task TopAsync()
        {
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();

            try
            {
                connection.Open();
                command.CommandText = "SELECT userid, coins FROM currency ORDER BY coins DESC LIMIT 5";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();

                List<string[]> users = new List<string[]>();

                while(coinsReader.Read())
                {
                    string userID = coinsReader["userid"].ToString();
                    string userCoins = coinsReader["coins"].ToString();

                    string[] thisUser = {userID, userCoins};

                    users.Add(thisUser);
                    
                }
                EmbedBuilder topUsers = new EmbedBuilder();
                var getColor = new Functions.RandomBuilderColor();

                topUsers.WithTitle("Quem está mais ligado a Deus?")
                    .WithColor(getColor.MessageBuilderColor());

                foreach (var user in users)
                {
                    string nick = "" + Context.Client.GetUser(ulong.Parse("" + user.GetValue(0)));
                    topUsers.AddField("" + nick, "Tem " + user.GetValue(1) + " de fé");
                    
                }

                await ReplyAsync("", false, topUsers);
                coinsReader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
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
