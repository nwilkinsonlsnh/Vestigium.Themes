# Vestigium.Themes

Modular WPF theming for the Vestigium suite. One control language, many palettes, runtime swap without rebuilding the host.

**Target:** .NET 10 / WPF / Visual Studio 2026  
**Package:** `Vestigium.Themes` 1.0.1 — one nupkg, eleven DLLs (core, catalog, nine palettes).  
**License:** MIT — [`LICENSE.md`](LICENSE.md)

Verbose reference: [`_Documentation/DevelopersGuide_v1.0.md`](_Documentation/DevelopersGuide_v1.0.md)  
Requirements: [`_Documentation/Requirements_v1.0.md`](_Documentation/Requirements_v1.0.md)

## Consume

```text
dotnet add package Vestigium.Themes --version 1.0.1
```

The package copies every palette DLL into the host output. You still **register** only the palettes the host will switch. NuGet does not call `Register` for you.

Call this in `OnStartup` **before** `base.OnStartup` / `StartupUri` parses the first window. `Initialize` merges metrics, the control catalog, then the requested palette.

```csharp
using Vestigium.Themes;

protected override void OnStartup(StartupEventArgs e)
{
    var manager = new ThemeManager();

    // Register every palette you will offer. Copy the lines you need from the table below.
    manager.Register(ThemeDefinition.FromPack(
        "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false,
        "Default Vestigium diagnostic chrome."));

    manager.Initialize(this, "LightBlue");
    base.OnStartup(e);
}
```

Later:

```csharp
manager.SwitchTheme("Nord");
```

Views keep explicit styles. Colors inside those styles use `{DynamicResource}`:

```xml
<Button Style="{StaticResource Button.Primary}" Content="Save" />
```

## Register each palette

`FromPack(id, displayName, assemblyName, isDark, description?)` builds:

`pack://application:,,,/{assemblyName};component/Themes/Theme.xaml`

The `id` must match `Vestigium.Theme.Id` inside that palette's `Theme.xaml`. Register each id once.

| Palette | Base | Register |
|---|---|---|
| Light Blue | Light | see below |
| Dark Mode | Dark | see below |
| Terminal | Dark | see below |
| Solarized Dark | Dark | see below |
| Standard WPF | Light | see below |
| Monokai | Dark | see below |
| Sublime (Mariana) | Dark | see below |
| Dracula | Dark | see below |
| Nord | Dark | see below |

```csharp
manager.Register(ThemeDefinition.FromPack(
    "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false,
    "Default Vestigium diagnostic chrome."));

manager.Register(ThemeDefinition.FromPack(
    "DarkMode", "Dark Mode", "Vestigium.Themes.DarkMode", isDark: true,
    "Low-glare dark surfaces."));

manager.Register(ThemeDefinition.FromPack(
    "Terminal", "Terminal", "Vestigium.Themes.Terminal", isDark: true,
    "Operator console."));

manager.Register(ThemeDefinition.FromPack(
    "SolarizedDark", "Solarized Dark", "Vestigium.Themes.SolarizedDark", isDark: true,
    "Calibrated Solarized Dark."));

manager.Register(ThemeDefinition.FromPack(
    "StandardWPF", "Standard WPF", "Vestigium.Themes.StandardWPF", isDark: false,
    "Platform-default approximation. Keys still resolve."));

manager.Register(ThemeDefinition.FromPack(
    "Monokai", "Monokai", "Vestigium.Themes.Monokai", isDark: true,
    "Wimer Monokai — pink accent on olive-black chrome."));

manager.Register(ThemeDefinition.FromPack(
    "Sublime", "Sublime", "Vestigium.Themes.Sublime", isDark: true,
    "Sublime Text Mariana — slate chrome, steel-blue accent."));

manager.Register(ThemeDefinition.FromPack(
    "Dracula", "Dracula", "Vestigium.Themes.Dracula", isDark: true,
    "Dracula — purple accent on midnight surfaces."));

manager.Register(ThemeDefinition.FromPack(
    "Nord", "Nord", "Vestigium.Themes.Nord", isDark: true,
    "Polar Night surfaces, Frost cyan accent."));
```

Register all nine if the host offers a full catalog. Register two if the product only switches Light Blue and Nord. Unregistered palettes are not loadable even though their DLLs are on disk.

`StandardWPF` is a palette. `ThemeManager.Unload()` is different — it drops Vestigium resources and `Button.Primary` stops resolving.

## Projects

| Project | Role |
|---|---|
| `Vestigium.Themes` | Contracts, `ThemeManager`, metrics |
| `Vestigium.Themes.Controls` | Explicit styles bound to tokens |
| Nine `Vestigium.Themes.*` palettes | XAML-only; identical keys, different values |
| `Vestigium.Themes.Pack` | Pack-only bag — produces the NuGet package |

## Pack (maintainers)

```powershell
dotnet pack src\Vestigium.Themes.Pack\Vestigium.Themes.Pack.csproj -c Release -o artifacts\nuget
```

Inspect `lib/net10.0-windows7.0/`. You want the eleven product DLLs and no `Vestigium.Themes.Pack.dll`.

```powershell
dotnet nuget push artifacts\nuget\Vestigium.Themes.1.0.1.nupkg `
  --api-key $env:VESTIGIUM_NUGET_APIKEY `
  --source https://api.nuget.org/v3/index.json
```

Do not commit the API key. Do not pack from `Vestigium.Themes.csproj` — core does not reference palettes.
