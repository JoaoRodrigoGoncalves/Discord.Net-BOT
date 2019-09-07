using Discord.Commands;
using System.IO;
using System;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class ShutUp : ModuleBase<SocketCommandContext>
    {
        [Command("shutup")]
        public async Task ShutUpAsync([Remainder] string user = null)
        {
            var date = DateTime.Now;
            string userId = null;

            if(user != null)
            {
                var charToRemove = new string[] { "<", "@", "!", ">" };
                foreach (var c in charToRemove)
                {
                    user = user.Replace(c, string.Empty);
                }
                userId = user;
            }
            else
            {
                userId = "" + Context.Message.Author.Id;
            }

            if (!Directory.Exists(@"C:\LeBotFiles\UserPreference"))
            {
                Directory.CreateDirectory(@"C:\LeBotFiles\UserPreference");
            }

            if (!Directory.Exists(@"C:\LeBotFiles\UserPreference\" + userId))
            {
                Directory.CreateDirectory(@"C:\LeBotFiles\UserPreference\" + userId);
            }

            if(!File.Exists(@"C:\LeBotFiles\UserPreference\" + userId + @"\botReadChat.cfg"))
            {
                string defaultConfig = "true";
                File.WriteAllText(@"C:\LeBotFiles\UserPreference\" + userId + @"\botReadChat.cfg", defaultConfig);
            }

            var thisFile = @"C:\LeBotFiles\UserPreference\" + userId + @"\botReadChat.cfg";
            int inUse = 1;
            int tryUse = 0;
            while (inUse != 0)
            {
                inUse = 0; //Valor aterado para 0 de maneira a ser alterado pelo try se necessário
                // Não é necessário colocar o 'tryUse = 0;' pois o mesmo é defenido como 0 antes do while() loop
                try
                {
                    string botReadConfig = File.ReadAllText(thisFile);
                    if(botReadConfig != "true")
                    {
                        File.WriteAllText(thisFile, "true");
                    }
                    else
                    {
                        File.WriteAllText(thisFile, "false");
                    }
                }
                catch (IOException)
                {
                    tryUse = 1;
                    inUse = 1;
                    Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Ficheiro " + thisFile + " está em uso.");
                }
                finally
                {
                    if (tryUse != 0 && inUse != 0)
                    {
                        Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] A tentar aceder ao ficheiro...");
                    }
                    else
                    {
                        inUse = 0;
                        Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Ficheiro " + thisFile + " desbloquado! A continuar...");
                    }
                }
            }

            await ReplyAsync("Configurações atualizadas!");
        }
    }
}
