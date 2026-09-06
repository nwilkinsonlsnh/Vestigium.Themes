namespace Vestigium.Themes;

public sealed record ThemeDefinition(
    string Id,
    string DisplayName,
    Uri ResourceUri,
    bool IsDark,
    string? Description = null)
{
    public static ThemeDefinition FromPack(
        string id,
        string displayName,
        string assemblyName,
        bool isDark,
        string? description = null,
        string relativePath = "Themes/Theme.xaml")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
        var uri = new Uri(
            $"pack://application:,,,/{assemblyName};component/{relativePath}",
            UriKind.Absolute);
        return new ThemeDefinition(id, displayName, uri, isDark, description);
    }
}
