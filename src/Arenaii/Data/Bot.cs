using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Arenaii.Data;

[DebuggerDisplay("{DebuggerDisplay}")]
[Serializable]
public class Bot : IComparable<Bot>
{
    public Bot()
    {
        Elo = 1600;
    }

    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("name")]
    public string? Name { get; set; }

    public string FullName
    {
        get
        {
            var name = Name;
            if (!string.IsNullOrEmpty(Version))
            {
                name += " v" + Version;
            }
            return name;
        }
    }

    [XmlAttribute("v")]
    public string? Version { get; set; }

    [XmlAttribute("elo")]
    public float Elo
    {
        get => (float)Math.Round((double)Rating, 1);
        set => Rating = value;
    }

    [XmlAttribute("a")]
    public bool Active { get; set; }

    [XmlIgnore]
    public Elo Rating { get; set; }

    [XmlIgnore]
    public FileInfo? Location { get; set; }

    public int CompareTo(Bot? other)
    {
        var compare = other?.Active.CompareTo(Active) ?? +1;
        if (compare != 0) { return compare; }
        return other!.Elo.CompareTo(Elo);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
    private string DebuggerDisplay
    {
        get
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0} v{1} {2:0.0} {{{3}}}{4}",
                Name,
                Version,
                Elo,
                Id,
                Active ? "*" : "");
        }
    }

    public static Bot? Create(DirectoryInfo directory)
    {
        var file = directory
            .EnumerateFiles()
            .FirstOrDefault(f => f.Name.ToUpperInvariant() == (directory.Name + ".EXE").ToUpperInvariant());

        if (file != null)
        {
            return Create(file);
        }
        file = directory.EnumerateFiles().FirstOrDefault(f => f.Extension == ".exe");
        if (file != null)
        {
            return Create(file);
        }
        return null;
    }

    public static Bot Create(FileInfo file)
    {
        Guard.Exists(file, "file");

        using var hasher = SHA1.Create();
        using var stream = file.OpenRead();
        var bot = new Bot()
        {
            Id = Convert.ToBase64String(hasher.ComputeHash(stream)).Replace("=", "").Replace("/", ""),
            Location = file,
        };

        try
        {
            var assembly = Assembly.LoadFile(file.FullName.Replace(".exe", ".dll"));
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            var versionObj = assembly.GetName().Version;
            var versionAtt = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            if (product != null)
            {
                bot.Name = product.Product;
            }
            if (versionAtt != null)
            {
                bot.Version = ToStrippedVersion(versionAtt.Version);
            }
            else if (versionObj?.ToString() != "0.0.0.0" && versionObj?.ToString() != "1.0.0.0")
            {
                bot.Version = ToStrippedVersion(versionObj!.ToString());
            }
        }
        catch { }

        if (string.IsNullOrEmpty(bot.Name))
        {
            bot.Name = Path.GetFileNameWithoutExtension(file.Name);
        }
        return bot;
    }

    private static string ToStrippedVersion(string version)
    {
        var parts = (version ?? string.Empty).Split('.').ToList();
        while (parts.Count > 1)
        {
            var last = parts[^1];
            if (last == "0" || last == "*")
            {
                parts.RemoveAt(parts.Count - 1);
            }
            else
            {
                break;
            }
        }
        return string.Join(".", parts);
    }
}
