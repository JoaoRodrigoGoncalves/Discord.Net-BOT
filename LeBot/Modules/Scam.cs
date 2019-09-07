using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Scam : ModuleBase<SocketCommandContext>
    {
        [Command("scam")]
        [Alias("steal", "roubar")]
        public async Task ScamAsync([Remainder] string user = null)
        {
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();

            if (user != null)
            {
                if (user.Contains("everyone"))
                {
                    return;
                }
                // Getting user id from mention
                var charToRemove = new string[] { "<", "@", "!", ">" };
                foreach (var c in charToRemove)
                {
                    user = user.Replace(c, string.Empty);
                }

                if(user == "" + Context.Message.Author.Id)
                {
                    await ReplyAsync("Burro do crl! Tás a tentar roubar-te a ti mesmo?");
                }
                else
                {
                    Random generateProbability = new Random();
                    int probability = generateProbability.Next(0, 101);

                    int bonus = 0;
                    var ownedItem = new Functions.OwnItem();
                    if(ownedItem.OwnedItem(Context.Message.Author.Id, 1))
                    {
                        bonus = 15;
                    }

                    int probabilidade = 15 + bonus;

                    if (probability >= probabilidade)
                    {
                        //The user was caught stealing!
                        await FailedAsync("" + Context.Message.Author.Id, user);
                    }
                    else
                    {
                        //The user wasn't caught stealing!
                        await SuccessfulAsync("" + Context.Message.Author.Id, user);
                        await Context.Message.DeleteAsync();
                    }
                }
            }
            else
            {
                //The user was caught stealing!
                await FailedAsync("" + Context.Message.Author.Id);
            }
        }

        //We should call this function if the user wasn't caught!
        public async Task SuccessfulAsync(string robber, string user)
        {
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            ulong coinsLadrao = 0;
            ulong coinsUser = 0;

            try
            {
                connection.Open();
                command.CommandText = "SELECT coins FROM currency WHERE userid ='" + robber + "' LIMIT 1";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();

                if (coinsReader.Read())
                {
                    coinsLadrao = coinsReader.GetUInt64("coins");
                    coinsReader.Close();
                }
                else
                {
                    coinsReader.Close();
                    coinsLadrao = 10;
                    command.CommandText = "INSERT INTO currency (userid) VALUES ('" + robber + "')";
                    command.ExecuteNonQuery();
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

            try
            {
                connection.Open();
                command.CommandText = "SELECT coins FROM currency WHERE userid ='" + user + "' LIMIT 1";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();

                if (coinsReader.Read())
                {
                    coinsUser = coinsReader.GetUInt64("coins");
                    coinsReader.Close();
                }
                else
                {
                    coinsReader.Close();
                    coinsUser = 10;
                    command.CommandText = "INSERT INTO currency (userid) VALUES ('" + user + "')";
                    command.ExecuteNonQuery();
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

            Random generatePercentage = new Random();
            double percentage = generatePercentage.NextDouble();

            if(percentage < 0.1)
            {
                percentage = 0.1;
            }

            ulong differenceCoins = (ulong)(coinsUser * percentage);

            ulong userAmount = coinsUser - differenceCoins;
            ulong robberAmount = coinsLadrao + differenceCoins;

            try
            {
                connection.Open();
                command.CommandText = "UPDATE currency SET coins='" + userAmount + "' WHERE userid='" + user + "' LIMIT 1";
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

            try
            {
                connection.Open();
                command.CommandText = "UPDATE currency SET coins='" + robberAmount + "' WHERE userid='" + robber + "' LIMIT 1";
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

            await ReplyAsync("Quem é que roubou " + differenceCoins + " de fé a " + Context.Client.GetUser(ulong.Parse(user)).Mention + "?");

        }

        //We should call this function if the user was caught!
        public async Task FailedAsync(string userFailed, string victim = null)
        {
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            ulong coinsLadrao = 0;

            try
            {
                connection.Open();
                command.CommandText = "SELECT coins FROM currency WHERE userid ='" + userFailed + "' LIMIT 1";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();

                if (coinsReader.Read())
                {
                    coinsLadrao = coinsReader.GetUInt64("coins");
                }
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

            Random percentagemRemovida = new Random();

            double percentagem = percentagemRemovida.NextDouble();

            if (percentagem < 0.1)
            {
                percentagem = 0.1;
            }

            ulong coinsRemovidas = (ulong)(coinsLadrao * percentagem);

            if(coinsRemovidas <= 0)
            {
                coinsRemovidas = 1;
            }

            ulong novototal = 0;

            if(coinsLadrao != 0)
            {
                novototal = coinsLadrao - coinsRemovidas;
                if (novototal <= 0)
                {
                    novototal = 0;
                }

                try
                {
                    connection.Open();
                    command.CommandText = "UPDATE currency SET coins='" + novototal + "' WHERE userid='" + userFailed + "' LIMIT 1";
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

                if(victim != null)
                {
                    ulong victimCoins = 0;
                    try
                    {
                        connection.Open();
                        command.CommandText = "SELECT coins FROM currency WHERE userid='" + victim +"' LIMIT 1";
                        MySqlDataReader coinsReader;
                        coinsReader = command.ExecuteReader();

                        if (coinsReader.Read())
                        {
                            victimCoins = coinsReader.GetUInt64("coins");
                        }
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

                    ulong victimAmout = victimCoins + coinsRemovidas;

                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='" + victimAmout + "' WHERE userid='" + victim + "' LIMIT 1";
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

                }
                else
                {

                    //454611732439236638 is BOT's userid

                    ulong victimCoins = 0;
                    try
                    {
                        connection.Open();
                        command.CommandText = "SELECT coins FROM currency WHERE userid='454611732439236638' LIMIT 1";
                        MySqlDataReader coinsReader;
                        coinsReader = command.ExecuteReader();

                        if (coinsReader.Read())
                        {
                            victimCoins = coinsReader.GetUInt64("coins");
                        }
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

                    ulong victimAmout = victimCoins + coinsRemovidas;

                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='" + victimAmout + "' WHERE userid='454611732439236638' LIMIT 1";
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
                }

                await ReplyAsync("Com que então ias roubar alguém " + Context.Client.GetUser(ulong.Parse(userFailed)).Mention + "? Só por causa dessa perdes " + coinsRemovidas + " de fé!");
            }
            else
            {
                Random dar = new Random();
                int veredito = dar.Next(1,3);
                if(veredito != 2)
                {
                    await ReplyAsync("Estás mesmo pobre " + Context.Client.GetUser(ulong.Parse(userFailed)).Mention + "... Pena que hoje não estou nos meus dias se não talvez te desse 1 de fé...");
                }
                else
                {
                    try
                    {
                        connection.Open();
                        command.CommandText = "UPDATE currency SET coins='1' WHERE userid='" + userFailed + "' LIMIT 1";
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
                    await ReplyAsync("Estás mesmo pobre " + Context.Client.GetUser(ulong.Parse(userFailed)).Mention + "... Toma 1 de fé e deixa-te de ser conas...");
                }
            }
        }
    }
}
