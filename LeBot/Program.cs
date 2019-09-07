/*
 * TODO:
 * -» Colocar um timer no Dab.cs para não puxar tanto pelos recursos do sistema ao tentar aceder á imagem;
 * -» Colocar um timer no Lapada.cd para não puxar tanto pelos recursos do sistema ao tentar aceder á imagem;
 */
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LeBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();


            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();


            string botToken = ""; //BOT Token

            //subscrição de eventos

            _client.Log += Log;
            _client.UserJoined += AnnounceUserJoined;
            //_client.UserUpdated += UpdateUserChange;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.StartAsync();

            await _client.SetGameAsync("\\help");

            await Task.Delay(-1);
        }

        /*private Task UpdateUserChange(SocketUser arg1, SocketUser arg2)
        {
            Console.WriteLine("Um utilizador levou update!");
            ulong userid = arg1.Id;
            string newName = arg2.Username;
            var connString = "Server=192.168.1.141;Port=3306;Database=lebot;Uid=lebot;Pwd=;SslMode=none;";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            try
            {
                connection.Open();
                command.CommandText = "SELECT username FROM user_username WHERE userid='" + userid + "' LIMIT 1";
                MySqlDataReader userReader;
                userReader = command.ExecuteReader();
                if (userReader.Read())
                {
                    //Utilizador já está registado
                    userReader.Close();
                    command.CommandText = "UPDATE user_ussername SET username='" + newName + "' WHERE userid='" + userid +"'";
                    command.ExecuteNonQuery();
                }
                else
                {
                    // Utilizador nunca registado
                    userReader.Close();
                    command.CommandText = "INSERT INTO user_username (userid, username) VALUES ('" + userid + "', '" + newName + "')";
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
            return Task.CompletedTask;
        }*/

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            await user.Guild.DefaultChannel.SendMessageAsync("Bem-vindo ao nosso servidor católico " + user.Mention);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var date = DateTime.Now;
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("\\", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) //Se o resultado não obtiver sucesso
                {
                    if (result.ErrorReason != "Unknown command.")
                    {
                        if (result.ErrorReason != "The input text has too many parameters.")
                        {
                            Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Erro \"" + result.ErrorReason + "\" por: " + message.Author.Username + "(" + message.Author.Discriminator + ")"); //Escreve o erro
                            await message.Channel.SendMessageAsync("Como eu sou uma merda mal feita, conseguiste provocar-me um AVC! Se eu continuar com AVCs, reporta o erro \"`" + result.Error + "` / `" + result.ErrorReason + "`\" juntamente com o comando completo que tentas-te executar!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] " + message.Author.Username + "(" + message.Author.Discriminator + ")" + " usou o comando \"" + message.Content + "\" no canal de text #" + message.Channel.Name); //Dar log no comando
                }
            }
            else
            {
                string botReadUserChat = "true";

                try
                {
                    if (!Directory.Exists(@"C:\LeBotFiles\UserPreference\"))
                    {
                        Directory.CreateDirectory(@"C:\LeBotFiles\UserPreference\");
                    }

                    if (!Directory.Exists(@"C:\LeBotFiles\UserPreference\" + message.Author.Id))
                    {
                        Directory.CreateDirectory(@"C:\LeBotFiles\UserPreference\" + message.Author.Id);
                        File.WriteAllText(@"C:\LeBotFiles\UserPreference\" + message.Author.Id + @"\botReadChat.cfg", "true");
                        botReadUserChat = "true";
                    }
                    else
                    {
                        botReadUserChat = File.ReadAllText(@"C:\LeBotFiles\UserPreference\" + message.Author.Id + @"\botReadChat.cfg");
                    }

                    if (botReadUserChat != "false")
                    {
                        string mString = message.Content;
                        bool noice = mString.Equals("noice", StringComparison.OrdinalIgnoreCase);
                        if (noice)
                        {
                            await message.Channel.SendMessageAsync("https://vignette.wikia.nocookie.net/kancolle/images/f/fd/Noice.png/revision/latest");
                            Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Noice a caminho de " + message.Author.Username + "#" + message.Author.Discriminator + "!");
                        }
                        bool boi = mString.Equals("boi", StringComparison.OrdinalIgnoreCase);
                        if (boi)
                        {
                            await message.Channel.SendMessageAsync("https://uproxx.files.wordpress.com/2018/04/god-of-war.jpg?quality=95&w=650");
                            Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Boi a caminho de " + message.Author.Username + "#" + message.Author.Discriminator + "!");
                        }
                        /*string[] badWords = { "fudi", "merda", "crl", "fds", "caralho", "cyka", "fodasse", "puta", "blayt", "cabrao", "foda", "shit", "fuck", "nigga", "quaralho", "cona", "piça", "picha", "merdita", "crlh", "foda-se", "caralhos", "vtf", "fuder", "fudeu", "fodeu", "fdd", "bullshit", "fdp", "bitch", "nigger" };
                        int NumBadWords = 0;
                        foreach (string x in badWords)
                        {
                            bool badWord = mString.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0;
                            if (badWord)
                            {
                                NumBadWords++;
                            }
                        }
                        if(mString.Equals("dasse") || mString.IndexOf(" dasse", StringComparison.OrdinalIgnoreCase) >= 0 || mString.IndexOf(" dasse ", StringComparison.OrdinalIgnoreCase) >= 0 || mString.IndexOf("dasse ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            NumBadWords++;
                        }
                        if (NumBadWords > 0)
                        {
                            await message.Channel.SendMessageAsync("Ó " + message.Author.Mention + " isto é um servidor católico caralho! Não se dizem asneiras boi.");
                        }
                        NumBadWords = 0;
                        */
                    }
                    else
                    {
                        Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] O bot não está autorizado a ler o chat de " + message.Author.Username + "#" + message.Author.Discriminator);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + message.Author.Username + "#" + message.Author.Discriminator + "gerou um(a) " + ex + "no canal de texto #" + message.Channel);
                }
            }
        }
    }
}
