using System;
using System.Windows.Markup;

namespace NitroxLauncher.LanguageLogic;

public class TranslatorExtension : MarkupExtension
{
    private string Key { get; }

    public TranslatorExtension(string key)
    {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Translator.Get(Key);
    }
}
