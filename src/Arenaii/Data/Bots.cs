using Arenaii.Configuration;
using System.IO;

namespace Arenaii.Data;

[Serializable]
public sealed class Bots : List<Bot>
{
    public Elo Rating => this.Average(bot => bot.Elo);

    public void Deactivate() { ForEach(bot => bot.Active = false); }

    public void Activate() { Activate(AppConfig.BotsDirectory); }

    public void Activate(DirectoryInfo directory)
    {
        Deactivate();

        foreach (var dir in directory.GetDirectories())
        {
            var bot = Bot.Create(dir);
            if (bot == null) { continue; }

            var existing = this.FirstOrDefault(b => b.Id == bot.Id);
            if (existing != null)
            {
                existing.Active = true;
                existing.Location = bot.Location;
            }
            else
            {
                bot.Active = true;
                Add(bot);
            }
        }
    }

    public Bot Get(string id)
    {
        return this.FirstOrDefault(bot => bot.Id == id);
    }
}
