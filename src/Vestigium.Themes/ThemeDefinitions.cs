namespace Vestigium.Themes;

/// <summary>
/// Shipped palette rows from README 1.0.2. Hosts register these. Package import does not.
/// </summary>
public static class ThemeDefinitions
{
    public static IReadOnlyList<ThemeDefinition> SuiteV1 { get; } =
    [
        ThemeDefinition.FromPack("LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", false, "Default Vestigium diagnostic chrome."),
        ThemeDefinition.FromPack("DarkMode", "Dark Mode", "Vestigium.Themes.DarkMode", true, "Low-glare dark surfaces."),
        ThemeDefinition.FromPack("Terminal", "Terminal", "Vestigium.Themes.Terminal", true, "Operator console."),
        ThemeDefinition.FromPack("SolarizedDark", "Solarized Dark", "Vestigium.Themes.SolarizedDark", true, "Calibrated Solarized Dark."),
        ThemeDefinition.FromPack("StandardWPF", "Standard WPF", "Vestigium.Themes.StandardWPF", false, "Platform-default approximation. Keys still resolve."),
        ThemeDefinition.FromPack("Monokai", "Monokai", "Vestigium.Themes.Monokai", true, "Wimer Monokai \u2014 pink accent on olive-black chrome."),
        ThemeDefinition.FromPack("Sublime", "Sublime", "Vestigium.Themes.Sublime", true, "Sublime Text Mariana \u2014 slate chrome, steel-blue accent."),
        ThemeDefinition.FromPack("Dracula", "Dracula", "Vestigium.Themes.Dracula", true, "Dracula \u2014 purple accent on midnight surfaces."),
        ThemeDefinition.FromPack("Nord", "Nord", "Vestigium.Themes.Nord", true, "Polar Night surfaces, Frost cyan accent."),
    ];
}
