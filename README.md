# Vestigium.Themes

Modular WPF theming for the Vestigium suite. One control language, many palettes, runtime swap without rebuilding the host.

**Target:** .NET 10 / WPF / Visual Studio 2026  
**Package:** `Vestigium.Themes` 1.0.0 — one nupkg, ten DLLs (core, catalog, eight palettes).

Verbose reference: [`_Documentation/DevelopersGuide_v1.0.md`](_Documentation/DevelopersGuide_v1.0.md)  
Requirements: [`_Documentation/Requirements_v1.0.md`](_Documentation/Requirements_v1.0.md)

## Consume

```text
dotnet add package Vestigium.Themes --version 1.0.0
```

The host still registers only the palettes it will switch. Unused palette DLLs sit in `bin`.

```csharp
var manager = new ThemeManager();
manager.Register(ThemeDefinition.FromPack(
    "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false));
manager.Initialize(Application.Current, "LightBlue");
// later
manager.SwitchTheme("Dracula");
```

Call `Initialize` in `OnStartup` before the first window is parsed. Views keep explicit styles. Color values inside those styles use `{DynamicResource}`:

```xml
<Button Style="{StaticResource Button.Primary}" Content="Save" />
```

## Pack (maintainers)

Libraries are not packable. Pack the bag project:

```powershell
dotnet pack src\Vestigium.Themes.Pack\Vestigium.Themes.Pack.csproj -c Release -o artifacts\nuget
```

Inspect `lib/net10.0-windows/` in the nupkg. You want the ten product DLLs and no `Vestigium.Themes.Pack.dll`.

```powershell
dotnet nuget push artifacts\nuget\Vestigium.Themes.1.0.0.nupkg `
  --api-key $env:VESTIGIUM_NUGET_APIKEY `
  --source https://api.nuget.org/v3/index.json
```

Do not commit the API key. Do not pack from `Vestigium.Themes.csproj` — core does not reference palettes.

## Projects

| Project | Role |
|---|---|
| `Vestigium.Themes` | Contracts, `ThemeManager`, metrics |
| `Vestigium.Themes.Controls` | Explicit styles bound to tokens |
| Eight `Vestigium.Themes.*` palettes | XAML-only; identical keys, different values |
| `Vestigium.Themes.Pack` | Pack-only bag — produces the NuGet package |

**StandardWPF** is a palette (keys still resolve). `ThemeManager.Unload()` is the escape hatch that returns stock WPF.

## Palettes

Light Blue · Dark Mode · Terminal · Solarized Dark · Standard WPF · Monokai · Sublime (Mariana) · Dracula
