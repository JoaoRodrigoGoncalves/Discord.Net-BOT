using Discord;
using Discord.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Shop : ModuleBase<SocketCommandContext>
    {
        [Command("shop")]
        [Alias("store")]
        public async Task ShopAsync(string order = "id", string mode = "ASC")
        {
            if (!order.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                if (!order.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    if (!order.Equals("price", StringComparison.OrdinalIgnoreCase))
                    {
                        await ReplyAsync(Context.Message.Author.Mention + "! Os únicos argumentos válidos para a ordenação dos itens da shop são `id`, `price` e `name`!");
                        return;
                    }
                }
            }

            if(!mode.Equals("ASC", StringComparison.OrdinalIgnoreCase))
            {
                if(!mode.Equals("DESC", StringComparison.OrdinalIgnoreCase))
                {
                    await ReplyAsync(Context.Message.Author.Mention + "! Os únicos argumentos válidos para o sentido da ordenação dos itens da shop são `ASC` e `DESC`!");
                    return;
                }
            }

            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();

            try
            {
                connection.Open();
                command.CommandText = "SELECT * FROM store_prices ORDER BY " + order + " " + mode + "";
                MySqlDataReader coinsReader;
                coinsReader = command.ExecuteReader();

                List<string[]> items = new List<string[]>();

                while (coinsReader.Read())
                {
                    string itemId = coinsReader["id"].ToString();
                    string itemName = coinsReader["name"].ToString();
                    string itemPrice = coinsReader["price"].ToString();
                    string itemDescription = coinsReader["description"].ToString();


                    string[] thisItem = { itemId, itemName, itemPrice, itemDescription };

                    items.Add(thisItem);

                }
                EmbedBuilder shopList = new EmbedBuilder();
                var getColor = new Functions.RandomBuilderColor();

                shopList.WithTitle("Loja d' *O Padre*")
                    .WithColor(getColor.MessageBuilderColor());

                foreach (var item in items)
                {
                    shopList.AddField("Id: " + item.GetValue(0) + " :arrow_right: " + item.GetValue(1), item.GetValue(3) + "\n" + "Preço: " + item.GetValue(2) + " de fé :pray:");

                }

                await ReplyAsync("", false, shopList);
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

            return;
        }
    }
}
