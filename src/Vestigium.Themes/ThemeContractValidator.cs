namespace Vestigium.Themes;

public static class ThemeContractValidator
{
    public static void Validate(ResourceDictionary dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        var missing = new List<string>();

        foreach (var key in ThemeResourceKeys.RequiredMetadata)
        {
            if (!dictionary.Contains(key)) missing.Add(key);
        }

        foreach (var key in ThemeResourceKeys.RequiredColors)
        {
            if (!dictionary.Contains(key)) missing.Add(key);
        }

        foreach (var key in ThemeResourceKeys.RequiredBrushes)
        {
            if (!dictionary.Contains(key)) missing.Add(key);
        }

        if (missing.Count > 0)
        {
            throw new ThemeLoadException(
                $"Theme is missing {missing.Count} required key(s): {string.Join(", ", missing)}")
            {
                MissingKeys = missing
            };
        }
    }
}
