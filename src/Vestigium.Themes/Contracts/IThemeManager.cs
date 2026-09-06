namespace Vestigium.Themes;

public interface IThemeManager
{
    IReadOnlyList<ThemeDefinition> AvailableThemes { get; }
    ThemeDefinition? Current { get; }

    void Register(ThemeDefinition theme);
    void Initialize(Application application, string? themeId = null);
    void SwitchTheme(string themeId);
    void Unload();

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}
