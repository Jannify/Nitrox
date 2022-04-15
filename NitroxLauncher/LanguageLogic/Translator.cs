using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NitroxModel.Helper;

namespace NitroxLauncher.LanguageLogic;

public static class Translator
{
    public static Language CurrentLanguage { get; private set; }
    private static readonly Language fallbackLanguage;
    private static readonly Dictionary<string, Language> languages = new();

    static Translator()
    {
        string languageFilesFolder = Path.Combine(NitroxUser.LauncherPath, "LanguageFiles", "Launcher");
        foreach (string languageFile in Directory.GetFiles(languageFilesFolder, "*.json", SearchOption.TopDirectoryOnly))
        {
            Dictionary<string, string> strings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(languageFile));
            string languageName = Path.GetFileNameWithoutExtension(languageFile);
            languages.Add(languageName, new Language(languageName, strings));
        }

        if (languages.TryGetValue("English", out Language language))
        {
            fallbackLanguage = language;
        }
        else
        {
            Log.Error($"Couldn't load fallback language at {Path.Combine(languageFilesFolder, "English.json")}");
        }

        SetLanguage(LauncherLogic.Config.Language);
    }

    public static string Get(string key)
    {
        if (!CurrentLanguage.TryGet(key, out string translation))
        {
            if (!fallbackLanguage.TryGet(key, out translation))
            {
                return $"{nameof(Translator)}:{key}";
            }
        }

        return translation;
    }

    public static void SetLanguage(string name)
    {
        if (languages.TryGetValue(name, out Language language))
        {
            CurrentLanguage = language;
        }
    }

    public static IEnumerable<string> GetSupportedLanguages() => new List<string>(languages.Keys);
}
