using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Missa : ModuleBase<SocketCommandContext>
    {
        [Command("missa")]
        [Alias("daily", "diario")]
        public async Task MissaAsync()
        {
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            ulong userReceivingCoins = 10;
            try
            {
                connection.Open();
                command.CommandText = "SELECT coins FROM currency WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();
                if (coinsReader.Read())
                {
                    userReceivingCoins = coinsReader.GetUInt32("coins");
                    coinsReader.Close();
                }
                else
                {
                    coinsReader.Close();
                    command.CommandText = "INSERT INTO currency (userid) VALUES ('" + Context.Message.Author.Id + "')";
                    command.ExecuteNonQuery();
                    userReceivingCoins = 10;
                }
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
            int lastday = 0;
            int lastmonth = 0;
            int lastyear = 0;
            bool exist = false;
            try
            {
                connection.Open();
                command.CommandText = "SELECT lastday, lastmonth, lastyear FROM missas WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                MySqlDataReader dateReader;
                dateReader = command.ExecuteReader();
                if (dateReader.Read())
                {
                    lastday = dateReader.GetInt32("lastday");
                    lastmonth = dateReader.GetInt32("lastmonth");
                    lastyear = dateReader.GetInt32("lastyear");
                    dateReader.Close();
                    exist = true;
                }
                else
                {
                    dateReader.Close();
                    command.CommandText = "INSERT INTO missas (userid, lastday, lastmonth, lastyear) VALUES ('" + Context.Message.Author.Id + "', '" + DateTime.Now.Day + "', '" + DateTime.Now.Month + "', '" + DateTime.Now.Year + "')";
                    command.ExecuteNonQuery();
                    exist = false;
                }
                connection.Close();

                //Check if bool "exist" was changed and if the user exists in the database
                bool allowed = false;
                if (exist)
                {
                    var checkTime = new Functions.TimeCalculation();
                    if(checkTime.TimeClaculations(lastday, lastmonth, lastyear))
                    {
                        allowed = true;
                    }
                }
                else
                {
                    allowed = true;
                }

                if (allowed)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(0, 26); //[0, 26[
                    if (randomNumber <= 0)
                    {
                        randomNumber = 1;
                    }
                    ulong userCoins = userReceivingCoins + Convert.ToUInt64(Convert.ToInt64(randomNumber));

                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='" + userCoins + "' WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                        command.ExecuteNonQuery();
                        connection.Close();

                        connection.Open();
                        command.CommandText = "UPDATE missas SET lastday='" + DateTime.Now.Day + "', lastmonth='" + DateTime.Now.Month + "', lastyear='" + DateTime.Now.Year + "' WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                        command.ExecuteNonQuery();
                        connection.Close();

                        EmbedBuilder ammountMessage = new EmbedBuilder();

                        var getColor = new Functions.RandomBuilderColor();
                        ammountMessage.WithTitle("Ganhas-te " + randomNumber + " de fé! :pray:")
                            .WithColor(getColor.MessageBuilderColor());

                        await ReplyAsync("", false, ammountMessage);
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
                else
                {
                    EmbedBuilder warningMessage = new EmbedBuilder();
                    warningMessage.WithTitle("Exedeste o teu limite diário!!")
                        .WithDescription("Tenta novamente amanhã para receberes a tua fé diária!");
                    await ReplyAsync("", false, warningMessage);
                }
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
