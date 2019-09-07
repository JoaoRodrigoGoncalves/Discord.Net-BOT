using Discord.Commands;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LeBot.Modules
{
    public class Lapada : ModuleBase<SocketCommandContext>
    {
        [Command("lapada")]
        public async Task LapadaAsync([Remainder] string slappedUser = null)
        {
            var date = DateTime.Now;
            if (slappedUser != null)
            {
                if (Context.Message.MentionedUsers.Count != 1)
                {
                    await ReplyAsync("ó poche, só mando lapadas a um de cada vez!");
                }
                else
                {
                    // Getting user id from mention
                    var charToRemove = new string[] { "<", "@", "!", ">" };
                    foreach (var c in charToRemove)
                    {
                        slappedUser = slappedUser.Replace(c, string.Empty);
                    }

                    string slappedAvatar = Context.Client.GetUser(ulong.Parse(slappedUser)).AvatarId;

                    using (WebClient client = new WebClient())
                    {
                        Console.Write(date.Hour + ":" + date.Minute + ":" + date.Second + " " + "[LeBot] A enviar pedido.");
                        client.DownloadFile(new Uri("http://192.168.1.141/bot/slap/?user=" + slappedUser + "&avatar=" + slappedAvatar), @"C:\LeBotFiles\Images\" + slappedUser + "_slap.png");
                        Console.WriteLine(" Pronto!");
                    }

                    var imageToSend = @"C:\LeBotFiles\Images\" + slappedUser + "_slap.png";
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
            else
            {
                await ReplyAsync("Ó palhaço, e dizeres quem é que vai mamar a lapada?");
            }
        }
    }
}
