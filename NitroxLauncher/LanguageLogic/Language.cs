using System.Collections.Generic;

namespace NitroxLauncher.LanguageLogic;

public class Language
{
    public string EnglishDisplayName { get; }
    private readonly Dictionary<string, string> entries;

    public Language(string name, Dictionary<string, string> entries)
    {
        EnglishDisplayName = name;
        this.entries = entries;
    }

    public bool TryGet(string key, out string translation) => entries.TryGetValue(key, out translation);
}
