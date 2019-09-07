using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Bet : ModuleBase<SocketCommandContext>
    {
        [Command("bet")]
        [Alias("apostar")]
        public async Task BetAsync([Remainder] string amount = null)
        {
            if(amount != null)
            {
                if (int.TryParse(amount, out int amountConverted))
                {
                    //Aqui você substitui pelos seus dados
                    var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
                    var connection = new MySqlConnection(connString);
                    var command = connection.CreateCommand();
                    var getColors = new Functions.RandomBuilderColor();

                    int currentCoins = 0;

                    try
                    {
                        connection.Open();
                        command.CommandText = "SELECT coins FROM currency WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                        MySqlDataReader coinsReader;
                        coinsReader = command.ExecuteReader();
                        if (coinsReader.Read())
                        {
                            currentCoins = coinsReader.GetInt32("coins");
                            coinsReader.Close();
                        }
                        else
                        {
                            coinsReader.Close();
                            command.CommandText = "INSERT INTO currency (userid) VALUES ('" + Context.Message.Author.Id + "') LIMIT 1";
                            command.ExecuteNonQuery();
                            currentCoins = 10;
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

                    Random chance = new Random();
                    int chanceResult = chance.Next(0,101);

                    /*
                     * 
                     * 0 - 50 -» Lost
                     * 51 - 90 -» Won x2
                     * 91 - 100 -» Won x3
                     * 
                     */
                    
                    int newTotal = 0;

                    if(chanceResult <= 50)
                    {
                        try
                        {
                            connection.Open();
                            command.CommandText = "UPDATE currency SET coins='" + (currentCoins-amountConverted) + "' WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                            command.ExecuteNonQuery();
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
                        await ReplyAsync(Context.Message.Author.Mention + " perdeste " + amountConverted + "...");
                    }
                    else
                    {
                        if (chanceResult >= 51 && chanceResult <= 90)
                        {
                            newTotal = amountConverted * 2;
                        }

                        if (chanceResult >= 91)
                        {
                            newTotal = amountConverted * 3;
                        }

                        int total = newTotal + currentCoins;

                        try
                        {
                            connection.Open();
                            command.CommandText = "UPDATE currency SET coins='" + total + "' WHERE userid='" + Context.Message.Author.Id + "' LIMIT 1";
                            command.ExecuteNonQuery();
                            await ReplyAsync(Context.Message.Author.Mention + ", ganhaste " + newTotal + "!");
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
                else
                {
                    await ReplyAsync("Números pah. Tu apostas números crl! Dasse " + Context.Message.Author.Mention + "...");
                }
            }
            else
            {
                await ReplyAsync("Ó " + Context.Message.Author.Mention + "! E dizeres quanto é que vais apostar?");
            }
        }
    }
}
