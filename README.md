# Vestigium.Themes

Modular WPF theming for the Vestigium suite. One control language, many palettes, runtime swap without rebuilding the host.

**Target:** .NET 10 / WPF / Visual Studio 2026  
**Startup project:** `Vestigium.Themes.Demo`

## Solution

| Project | Role |
|---|---|
| `Vestigium.Themes` | Contracts, `ThemeManager`, metrics |
| `Vestigium.Themes.Controls` | Explicit styles (`Button.Primary`, …) bound with `{DynamicResource}` |
| `Vestigium.Themes.LightBlue` | Palette — Fluent-inspired light |
| `Vestigium.Themes.DarkMode` | Palette — low-glare dark |
| `Vestigium.Themes.Terminal` | Palette — operator console |
| `Vestigium.Themes.SolarizedDark` | Palette — Schoonover Solarized Dark |
| `Vestigium.Themes.StandardWPF` | Palette — Windows chrome hues, same keys |
| `Vestigium.Themes.Monokai` | Palette — Wimer Monokai |
| `Vestigium.Themes.Sublime` | Palette — Sublime Text Mariana |
| `Vestigium.Themes.Dracula` | Palette — Dracula |
| `Vestigium.Themes.Demo` | Control gallery + switcher |

Theme packages are XAML-only. The host registers the packages it references.

## Host integration

```csharp
var manager = new ThemeManager();
manager.Register(ThemeDefinition.FromPack(
    "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false));
manager.Initialize(Application.Current, "LightBlue");
// later
manager.SwitchTheme("Dracula");
```

`ThemeManager` replaces **only** the palette dictionary. Metrics and control styles stay merged.

Views keep explicit styles:

```xml
<Button Style="{StaticResource Button.Primary}" Content="Save" />
```

`StaticResource` on the style key is correct. Color values inside those styles use `DynamicResource`, so a palette swap repaints the tree.

## Standard WPF vs unload

- **StandardWPF** — Vestigium style keys still exist; hues approximate Windows chrome / `SystemColors`.
- **`ThemeManager.Unload()`** — removes palettes and control dictionaries. Stock WPF returns. `Button.Primary` will not resolve.

## Contract

Every `Theme.xaml` must supply the keys in `ThemeResourceKeys`. `ThemeContractValidator` throws `ThemeLoadException` listing missing keys.

## Pack URIs

```
pack://application:,,,/Vestigium.Themes.{Id};component/Themes/Theme.xaml
pack://application:,,,/Vestigium.Themes.Controls;component/Themes/Controls.xaml
```
