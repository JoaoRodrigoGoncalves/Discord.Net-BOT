using Discord.Commands;
using MySql.Data.MySqlClient;using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Buy : ModuleBase<SocketCommandContext>
    {
        [Command("buy")]
        [Alias("comprar")]
        public async Task BuyAsync([Remainder] string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                await ReplyAsync("E dizeres o que é que queres comprar?");
                return;
            }
            else
            {
                ulong.TryParse(id, out ulong itemId);
                if (itemId <= 0)
                {
                    await ReplyAsync("Na minha terra um id são números...");
                    return;
                }
                else
                {
                    int newitemId = int.Parse("" + itemId);
                    //Aqui você substitui pelos seus dados
                    var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
                    var connection = new MySqlConnection(connString);
                    var command = connection.CreateCommand();

                    connection.Open();
                    command.CommandText = "SELECT * FROM store_prices WHERE id='" + itemId + "' LIMIT 1";
                    MySqlDataReader itemReader;
                    itemReader = command.ExecuteReader();
                    if (itemReader.Read())
                    {
                        long price = itemReader.GetInt64(2);
                        string itemName = itemReader.GetString(1);
                        itemReader.Close();

                        command.CommandText = "SELECT coins FROM currency WHERE userid='" + Context.Message.Author.Id + "'";
                        MySqlDataReader coinsReader;
                        coinsReader = command.ExecuteReader();
                        long userCoins;
                        if (coinsReader.Read())
                        {
                            userCoins = coinsReader.GetInt64(0);
                        }
                        else
                        {
                            userCoins = 10;
                        }
                        coinsReader.Close();

                        if(price > userCoins)
                        {
                            await ReplyAsync("Não tens fé suficiente :neutral_face:");
                        }
                        else
                        {
                            var testItem = new Functions.OwnItem();
                            if(!testItem.OwnedItem(Context.Message.Author.Id, newitemId))
                            {
                                long newTotal = userCoins - price;
                                command.CommandText = "UPDATE currency SET coins='" + newTotal + "' WHERE userid='" + Context.Message.Author.Id + "'";
                                command.ExecuteNonQuery();
                                command.CommandText = "INSERT INTO user_inventory (userid, itemid) VALUES ('" + Context.Message.Author.Id + "', '" + newitemId + "')";
                                command.ExecuteNonQuery();
                                await ReplyAsync(Context.Message.Author.Mention + " adquiriste " + itemName + "!");
                            }
                            else
                            {
                                await ReplyAsync("Já tens esse item no teu inventario " + Context.Message.Author.Mention + "! Usa \\inventario para ver os teus itens!");
                            }
                            
                        }
                        connection.Close();
                    }
                    else
                    {
                        itemReader.Close();
                        connection.Close();
                        await ReplyAsync("Esse item não existe :neutral_face:");
                        return;
                    }
                }
            }
        }
    }
}
