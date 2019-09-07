using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Fireworks : ModuleBase<SocketCommandContext>
    {
        [Command("festa")]
        [Alias("foguetes", "fireworks")]
        public async Task FireworksAsync()
        {
            var date = DateTime.Now;
            if (!Directory.Exists(@"C:\LeBotFiles\Images\"))
            {
                Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Caminho C:\\LeBotFiles\\Images\\ não existe. A criar...");
                Directory.CreateDirectory(@"C:\LeBotFiles\Images\");
            }

            using (WebClient client = new WebClient())
            {
                Console.Write(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] A enviar pedido.");
                client.DownloadFile(new Uri("http://192.168.1.141/bot/fireworks/?user=" + Context.Message.Author.Id + "&avatar=" + Context.Message.Author.AvatarId), @"C:\LeBotFiles\Images\" + Context.Message.Author.Id + "_fireworks.png");
                Console.WriteLine(" Pronto!");
            }

            var imageToSend = @"C:\LeBotFiles\Images\" + Context.Message.Author.Id + "_fireworks.png";
            int inUse = 1;
            int tryUse = 0;
            while (inUse != 0)
            {
                inUse = 0; //Valor aterado para 0 de maneira a ser alterado pelo try se necessário
                // Não é necessário colocar o 'tryUse = 0;' pois o mesmo é defenido como 0 antes do while() loop
                try
                {
                    await Context.Channel.SendFileAsync(imageToSend);
                }
                catch (IOException)
                {
                    tryUse = 1;
                    inUse = 1;
                    Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Ficheiro " + imageToSend + " está em uso.");
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
                        Console.WriteLine(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] Ficheiro " + imageToSend + " desbloquado! A continuar...");
                    }
                }
            }
        }
    }
}
