using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Inventory : ModuleBase<SocketCommandContext>
    {
        [Command("inventario")]
        [Alias("itens", "inventory")]
        public async Task InventoryAsync()
        {
            ulong userid = Context.Message.Author.Id;
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            List<string[]> items = new List<string[]>();
            List<string[]> userItems = new List<string[]>();

            connection.Open();
            command.CommandText = "SELECT * FROM store_prices";
            MySqlDataReader itemReader;
            itemReader = command.ExecuteReader();
            try
            {
                while (itemReader.Read())
                {
                    string itemId = itemReader["id"].ToString();
                    string itemName = itemReader["name"].ToString();
                    string itemPrice = itemReader["price"].ToString();
                    string itemDescription = itemReader["description"].ToString();

                    string[] thisItem = { itemId, itemName, itemPrice, itemDescription };
                    
                    items.Add(thisItem);
                }
                itemReader.Close();
                connection.Close();

                var ownItem = new Functions.OwnItem();

                foreach (var item in items)
                {
                    if (ownItem.OwnedItem(userid, int.Parse("" + item.GetValue(0))))
                    {
                        string[] thisUserItem = { "" + item.GetValue(0), "" + item.GetValue(1), "" + item.GetValue(2), "" + item.GetValue(3) };
                        userItems.Add(thisUserItem);
                    }
                }

                EmbedBuilder inventoryDump = new EmbedBuilder();
                var getColor = new Functions.RandomBuilderColor();

                inventoryDump.WithTitle("Inventário de " + Context.Message.Author.Username + "#" + Context.Message.Author.Discriminator);
                inventoryDump.WithColor(getColor.MessageBuilderColor());

                if (userItems.Count > 0)
                {
                    foreach (var ownedItem in userItems)
                    {
                        inventoryDump.AddField("Id: " + ownedItem.GetValue(0) + " :arrow_right: " + ownedItem.GetValue(1), ownedItem.GetValue(3));
                    }
                }
                else
                {
                    inventoryDump.WithDescription("Nada... Talvez usar a \\shop para encontrar algo?");
                }
                await ReplyAsync("", false, inventoryDump);
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
