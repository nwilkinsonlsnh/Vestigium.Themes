namespace Vestigium.Themes;

/// <summary>
/// Shipped palette rows. Hosts register these. Package import does not.
/// </summary>
public static class ThemeDefinitions
{
    public static IReadOnlyList<ThemeDefinition> SuiteV1 { get; } =
    [
        ThemeDefinition.FromPack("LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", false, "Default Vestigium diagnostic chrome."),
        ThemeDefinition.FromPack("DarkMode", "Dark Mode", "Vestigium.Themes.DarkMode", true, "Low-glare dark surfaces."),
        ThemeDefinition.FromPack("Terminal", "Terminal", "Vestigium.Themes.Terminal", true, "Operator console."),
        ThemeDefinition.FromPack("SolarizedDark", "Solarized Dark", "Vestigium.Themes.SolarizedDark", true, "Calibrated Solarized Dark."),
        ThemeDefinition.FromPack("StandardWPF", "Standard WPF", "Vestigium.Themes.StandardWPF", false, "Platform-default approximation."),
        ThemeDefinition.FromPack("Monokai", "Monokai", "Vestigium.Themes.Monokai", true, "Pink accent on olive-black chrome."),
        ThemeDefinition.FromPack("Sublime", "Sublime", "Vestigium.Themes.Sublime", true, "Mariana slate chrome."),
        ThemeDefinition.FromPack("Dracula", "Dracula", "Vestigium.Themes.Dracula", true, "Purple accent on midnight surfaces."),
        ThemeDefinition.FromPack("Nord", "Nord", "Vestigium.Themes.Nord", true, "Polar Night surfaces, Frost cyan accent."),
    ];
}
