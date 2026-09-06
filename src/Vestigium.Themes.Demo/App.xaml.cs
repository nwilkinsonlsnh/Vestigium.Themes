using Microsoft.Extensions.DependencyInjection;
using Vestigium.Themes;

namespace Vestigium.Themes.Demo;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var manager = new ThemeManager();
        manager.Register(ThemeDefinition.FromPack("LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", false,
            "Default Vestigium diagnostic chrome."));
        manager.Register(ThemeDefinition.FromPack("DarkMode", "Dark Mode", "Vestigium.Themes.DarkMode", true,
            "Low-glare dark surfaces."));
        manager.Register(ThemeDefinition.FromPack("Terminal", "Terminal", "Vestigium.Themes.Terminal", true,
            "Operator console."));
        manager.Register(ThemeDefinition.FromPack("SolarizedDark", "Solarized Dark", "Vestigium.Themes.SolarizedDark", true,
            "Calibrated Solarized Dark."));
        manager.Register(ThemeDefinition.FromPack("StandardWPF", "Standard WPF", "Vestigium.Themes.StandardWPF", false,
            "Platform-default approximation."));
        manager.Register(ThemeDefinition.FromPack("Monokai", "Monokai", "Vestigium.Themes.Monokai", true,
            "Wimer Monokai — pink accent on olive-black chrome."));
        manager.Register(ThemeDefinition.FromPack("Sublime", "Sublime", "Vestigium.Themes.Sublime", true,
            "Sublime Text Mariana — slate chrome, steel-blue accent."));
        manager.Register(ThemeDefinition.FromPack("Dracula", "Dracula", "Vestigium.Themes.Dracula", true,
            "Dracula — purple accent on midnight surfaces."));

        manager.Initialize(this, "LightBlue");

        var services = new ServiceCollection();
        services.AddSingleton<IThemeManager>(manager);
        services.AddSingleton<MainViewModel>();
        Services = services.BuildServiceProvider();

        base.OnStartup(e);
    }
}
