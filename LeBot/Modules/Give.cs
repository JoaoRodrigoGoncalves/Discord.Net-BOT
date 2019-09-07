using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Give : ModuleBase<SocketCommandContext>
    {
        [Command("give")]
        public async Task giveAsync(uint quantity = 0, [Remainder] string userReceiving = null)
        {
            if (quantity < 1)
            {
                await ReplyAsync("Rlly nigga?");
                return;
            }
            if(userReceiving != null)
            {
                string originalUserReceiving = userReceiving;
                var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
                var connection = new MySqlConnection(connString);
                var command = connection.CreateCommand();
                uint userSendingCoins = 0;
                uint userReceivingCoins = 0;
                string userReceivingid = null;
                ulong userSendingid = Context.Message.Author.Id;
                var charToRemove = new string[] { "<", "@", "!", ">" };

                foreach (var c in charToRemove)
                {
                    userReceiving = userReceiving.Replace(c, string.Empty);
                }

                userReceivingid = userReceiving;

                // Getting the amount of credits that the recieving user currently has
                try
                {
                    connection.Open();
                    command.CommandText = "SELECT coins FROM currency WHERE userid='" + userReceivingid + "' LIMIT 1";
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
                        command.CommandText = "INSERT INTO currency (userid) VALUES ('" + userReceivingid + "')";
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
                //end of trying to get the amount of credits that the recieving user currently has

                // Getting the amout of credits that the Sender user currently has
                try
                {
                    connection.Open();
                    command.CommandText = "SELECT coins FROM currency WHERE userid='" + userSendingid + "' LIMIT 1";
                    MySqlDataReader coinsReader;
                    coinsReader = command.ExecuteReader();
                    if (coinsReader.Read())
                    {
                        userSendingCoins = coinsReader.GetUInt32("coins");
                        coinsReader.Close();
                    }
                    else
                    {
                        coinsReader.Close();
                        command.CommandText = "INSERT INTO currency (userid) VALUES ('" + userSendingid + "')";
                        command.ExecuteNonQuery();
                        userSendingCoins = 10;
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
                //End of trying to get the amount of credits that each user currently has

                /*
                 * User wich is going to send credits balance -» userSendingCoins
                 * User wich is going to receive credits balance -» userReceivingCoins
                 * Amout of credits that the sender user wish to send -» quantity
                 */

                // Checking if the the Sender has enough credits in his balance to send to the Receiver
                if (userSendingCoins >= quantity)
                {
                    ulong newSendigUserTotal = userSendingCoins - quantity;
                    ulong newReceivingUserTotal = userReceivingCoins + quantity;

                    // Applying the new balance to the Sender
                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='" + newSendigUserTotal + "' WHERE userid='" + userSendingid + "' LIMIT 1";
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

                    //Applying the new balance to the Receiver
                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='" + newReceivingUserTotal + "' WHERE userid='" + userReceivingid + "' LIMIT 1";
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
                    await ReplyAsync(Context.Message.Author.Mention + " deu " + quantity + " de fé a " + originalUserReceiving + "!");
                }
                else
                {
                    await ReplyAsync("Ó " + Context.Message.Author.Mention + " tu não tens " + quantity + " de fé!");
                }
            }
            else
            {
                await ReplyAsync("Ó " + Context.Message.Author.Mention + " e dizeres quem é que vai receber?");
            }
            
        }
    }
}
