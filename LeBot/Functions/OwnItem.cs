using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeBot.Functions
{
    class OwnItem
    {
        public bool OwnedItem(ulong userid, int itemId)
        {
            //Aqui você substitui pelos seus dados
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();

            connection.Open();
            command.CommandText = "SELECT itemid FROM user_inventory WHERE userid = '" + userid + "' AND itemid='" + itemId + "' LIMIT 1";
            MySqlDataReader itemReader;
            itemReader = command.ExecuteReader();
            if (itemReader.Read())
            {
                itemReader.Close();
                connection.Close();
                return true;
            }
            else
            {
                itemReader.Close();
                connection.Close();
                return false;
            }
        }
    }
}
