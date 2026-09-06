# Vestigium.Themes

Modular WPF theming for the Vestigium suite. One control language, many palettes, runtime swap without rebuilding the host.

**Target:** .NET 10 / WPF / Visual Studio 2026  
**Startup project:** `Vestigium.Themes.Demo`

Verbose reference: [`_Documents/DevelopersGuide.md`](_Documents/DevelopersGuide.md)

## Open the demo

1. Clone this repository.
2. Open `Vestigium.Themes.slnx` in Visual Studio 2026.
3. Set **Vestigium.Themes.Demo** as the startup project.
4. Restore NuGet (`CommunityToolkit.Mvvm`, `Microsoft.Extensions.DependencyInjection`).
5. Run on Windows.

The demo is a **TabControl** gallery: buttons, data entry, PingIQ / DnsIQ DataGrids, TraceIQ hops, navigation chrome, and a console log. Switch palettes from the header combo — style keys stay the same.

## Host in three calls

```csharp
var manager = new ThemeManager();
manager.Register(ThemeDefinition.FromPack(
    "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false));
manager.Initialize(Application.Current, "LightBlue");
// later
manager.SwitchTheme("Dracula");
```

Views keep explicit styles. Color values inside those styles use `{DynamicResource}`:

```xml
<Button Style="{StaticResource Button.Primary}" Content="Save" />
```

## Projects

| Project | Role |
|---|---|
| `Vestigium.Themes` | Contracts, `ThemeManager`, metrics |
| `Vestigium.Themes.Controls` | Explicit styles bound to tokens |
| Eight `Vestigium.Themes.*` palettes | XAML-only; identical keys, different values |
| `Vestigium.Themes.Demo` | Tabbed control gallery + switcher |

**StandardWPF** is a palette (keys still resolve). `ThemeManager.Unload()` is the escape hatch that returns stock WPF.

## Palettes

Light Blue · Dark Mode · Terminal · Solarized Dark · Standard WPF · Monokai · Sublime (Mariana) · Dracula
