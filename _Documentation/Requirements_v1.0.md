# Vestigium.Themes — Requirements Specification v1.0

**Product:** Vestigium.Themes  
**Contract:** 1.0  
**Platform:** .NET 10 / WPF / Visual Studio 2026  
**Status:** Implemented in this repository  

Companion documents:

- Quickstart — [`README.md`](../README.md)
- Host / catalog reference — [`DevelopersGuide_v1.0.md`](DevelopersGuide_v1.0.md)

This specification states **what the library must do**. How to consume it is in the Developers Guide.

---

## 1. Purpose

Vestigium.Themes is the shared visual language for the Vestigium diagnostic suite (PingIQ, DnsIQ, TraceIQ, and future instruments). Hosts must be able to:

1. Style WPF controls with **explicit, named styles** (`Button.Primary`, `DataGrid.Standard`, …).
2. Swap **palettes at runtime** without rebuilding the host or reloading windows.
3. Ship only the palettes a given product needs.
4. Fail fast when a palette is incomplete.

The library is **not** a general-purpose Fluent/MahApps replacement. It is a locked catalog plus interchangeable color packages.

---

## 2. Scope

### 2.1 In scope (v1.0)

| ID | Capability |
|----|------------|
| S-01 | Core contracts, `ThemeManager`, metrics, converters |
| S-02 | Explicit control catalog (`Vestigium.Themes.Controls`) |
| S-03 | Eight palettes with an identical resource-key contract |
| S-04 | Runtime theme switch that replaces only the palette dictionary |
| S-05 | Contract validation on every palette load |
| S-06 | Standard WPF palette (platform-like hues, keys still resolve) |
| S-07 | `Unload()` to drop Vestigium resources entirely |
| S-08 | Tabbed Demo that exercises the catalog and diagnostic mockups |
| S-09 | Solution format `Vestigium.Themes.slnx` for Visual Studio 2026 |

### 2.2 Out of scope (v1.0)

- NuGet packing / private feed publishing
- Implicit `TargetType` styles (stock WPF look when a view omits `Style=`)
- Skia / WinUI / MAUI / web ports of the catalog
- Live theme authoring UI inside the WPF Demo
- Localization of style keys or palette display names beyond `en-US` metadata
- Custom control assemblies beyond restyling native WPF types
- Authentication, licensing, or telemetry

---

## 3. Stakeholders

| Role | Need |
|------|------|
| Instrument host (PingIQ / DnsIQ / TraceIQ) | Register palettes, initialize once, style views with catalog keys |
| Visual designer | One catalog, many palettes; tokens named by role not by hex |
| Library maintainer | Single key contract; adding a palette is copy + retint |
| Operator / QA | Demo that proves every shipped style under every palette |

---

## 4. Platform requirements

| ID | Requirement |
|----|-------------|
| PL-01 | Target framework `net10.0-windows` with `UseWPF`. |
| PL-02 | Solution opens in Visual Studio 2026 as `Vestigium.Themes.slnx`. |
| PL-03 | Language version latest C#; nullable enabled. |
| PL-04 | Demo may depend on `CommunityToolkit.Mvvm` 8.4.0 and `Microsoft.Extensions.DependencyInjection` 10.0.0. Core and palettes shall not take those packages. |
| PL-05 | Palette projects are XAML-only class libraries (`EnableDefaultCompileItems=false`) so pack URIs resolve. |
| PL-06 | Pack URI shape is `pack://application:,,,/Vestigium.Themes.{Id};component/Themes/Theme.xaml`. |
| PL-07 | Neutral language `en-US`. |

---

## 5. Functional requirements

### 5.1 Architecture

| ID | Requirement |
|----|-------------|
| FR-A01 | The solution SHALL split into core (`Vestigium.Themes`), controls (`Vestigium.Themes.Controls`), one project per palette, and `Vestigium.Themes.Demo`. |
| FR-A02 | Core SHALL NOT project-reference any palette assembly. The host registers packages it references. |
| FR-A03 | Control styles SHALL live in Controls, not copied into each palette. |
| FR-A04 | Palettes SHALL supply only tokens (`Vestigium.Theme.*`, `Vestigium.Colors.*`, `Vestigium.Brushes.*`). |
| FR-A05 | Metrics (fonts, radii, spacing) SHALL live in core `Metrics.xaml` and SHALL remain merged across palette swaps. |

### 5.2 Theme manager

| ID | Requirement |
|----|-------------|
| FR-M01 | `IThemeManager` SHALL expose `Register`, `Initialize`, `SwitchTheme`, `Unload`, `AvailableThemes`, `Current`, and `ThemeChanged`. |
| FR-M02 | `Initialize(Application, themeId)` SHALL merge Metrics, then Controls, then the requested palette, **before** the first window is parsed. |
| FR-M03 | `Initialize` SHALL NOT call `Application.Resources.Clear()`. Host resources already merged MUST remain. |
| FR-M04 | `SwitchTheme(id)` SHALL validate the palette, remove merged dictionaries that contain `Vestigium.Theme.Id`, add the new palette, and raise `ThemeChanged`. |
| FR-M05 | `Register` of a duplicate `ThemeDefinition.Id` SHALL replace the previous definition or throw — the implementation MUST be deterministic and documented. Current behavior: last register for an id wins by throwing `InvalidOperationException` if already registered. Hosts MUST register each id once. |
| FR-M06 | Unknown `themeId` on `SwitchTheme` SHALL throw. |
| FR-M07 | `Unload()` SHALL remove Vestigium metrics, controls, and palette dictionaries so stock WPF styles apply. Catalog keys (`Button.Primary`) SHALL then fail to resolve. |
| FR-M08 | `ThemeDefinition.FromPack(id, displayName, assemblyName, isDark, description?)` SHALL build the pack URI in FR/PL-06. |

### 5.3 Resource contract

| ID | Requirement |
|----|-------------|
| FR-C01 | `ThemeResourceKeys` is the locked v1.0 list. Additive keys require a contract version bump. |
| FR-C02 | Every palette `Theme.xaml` SHALL provide all required metadata, color, and brush keys. |
| FR-C03 | Required metadata: `Vestigium.Theme.Id`, `.DisplayName`, `.Base` (`Light` or `Dark`), `.IsDark`, `.Version`, `.Showcase` (a `Color`). `.Description` is optional. |
| FR-C04 | Color keys are `Color`. Brush keys are `SolidColorBrush` whose `Color` is `{StaticResource}` of the matching color key (except `Vestigium.Brushes.Overlay.Scrim`, which has no color twin). |
| FR-C05 | `ThemeContractValidator.Validate` SHALL throw `ThemeLoadException` with `MissingKeys` populated when any required key is absent. |
| FR-C06 | `SwitchTheme` and `Initialize` SHALL run the validator before the palette becomes current. |
| FR-C07 | Styles SHALL bind paints with `{DynamicResource Vestigium.Brushes.*}` (and `{DynamicResource Vestigium.Colors.*}` only on properties of type `Color`, e.g. `DropShadowEffect.Color`). |
| FR-C08 | Views SHALL apply catalog styles with `{StaticResource Button.Primary}` etc. Views SHALL NOT hard-code palette hex. |

### 5.4 Control catalog

| ID | Requirement |
|----|-------------|
| FR-S01 | No implicit `TargetType` styles in the catalog. Omitting `Style=` yields stock WPF, not a silent Vestigium default. |
| FR-S02 | Public keys follow `{Control}.{Variant}` (`Button.Primary`, `DataGrid.Compact`, `TabItem.Pill`). |
| FR-S03 | Variants express **intent**: Primary, Secondary, Success, Warning, Danger, Utility, Error, Standard, Compact, Card, Flush. |
| FR-S04 | Interactive controls SHALL include hover, pressed, disabled, and visible keyboard-focus visuals. |
| FR-S05 | `Controls.xaml` SHALL merge `ScrollBar.Styles.xaml` **before** `ScrollViewer.Styles.xaml`. |
| FR-S06 | Duplicate `x:Key` across style files is forbidden (last-writer-wins is a defect). |
| FR-S07 | ToolBar children that are buttons SHALL set `Style="{StaticResource Button.*}"` explicitly so `ToolBar.ButtonStyleKey` does not override the catalog. |
| FR-S08 | Status presentation in grids SHALL use `DataTrigger` → `Vestigium.Brushes.Status.Success\|Warning\|Error`, not a single static ellipse on every row. |
| FR-S09 | Warning accent token is `#D83B01` in Light Blue (not Ellipse orange `#FF8C00`). Other palettes retint the same key. |
| FR-S10 | Fonts: `Vestigium.Fonts.Family.UI` = Segoe UI Variable / Segoe UI / Tahoma; `Vestigium.Fonts.Family.Mono` = Cascadia Mono / Consolas / Courier New. No CSS generic families (`sans-serif`). |

### 5.5 Palettes (v1.0)

Each palette is a separate assembly. All MUST satisfy FR-C02.

| ID | Assembly | Base | Accent intent |
|----|----------|------|----------------|
| TH-01 | `Vestigium.Themes.LightBlue` | Light | Default diagnostic chrome (`#0078D7`) |
| TH-02 | `Vestigium.Themes.DarkMode` | Dark | Low-glare IDE-like surfaces |
| TH-03 | `Vestigium.Themes.Terminal` | Dark | Operator console / phosphor |
| TH-04 | `Vestigium.Themes.SolarizedDark` | Dark | Calibrated Solarized Dark |
| TH-05 | `Vestigium.Themes.StandardWPF` | Light | SystemColors-like hues; **still a palette** |
| TH-06 | `Vestigium.Themes.Monokai` | Dark | Wimer Monokai (`#F92672` on `#272822`) |
| TH-07 | `Vestigium.Themes.Sublime` | Dark | Sublime Text **Mariana** (`#6699CC` on `#303841`), not a second Monokai |
| TH-08 | `Vestigium.Themes.Dracula` | Dark | Dracula (`#BD93F9` on `#282A36`) |

| ID | Requirement |
|----|-------------|
| FR-P01 | `Vestigium.Theme.Id` SHALL match the `FromPack` id (e.g. `LightBlue`, `SolarizedDark`, `StandardWPF`). |
| FR-P02 | Primary-button foreground uses `Text.Inverse`. Inverse MUST contrast with `Accent.Primary` (dark inverse on light accents such as Terminal green / Dracula purple). |
| FR-P03 | `#FFFFFF` in source catalogs is dual-use: inverse text vs card fill. Mapping is property-aware (Foreground/Caret/Stroke/Fill → `Text.Inverse`; Background/BorderBrush → `Surface.Card`). |
| FR-P04 | StandardWPF SHALL keep every contract key. It is not an Unload. |

### 5.6 Demo

| ID | Requirement |
|----|-------------|
| FR-D01 | Startup project is `Vestigium.Themes.Demo` (`WinExe`). |
| FR-D02 | Demo SHALL register all eight palettes and initialize to Light Blue. |
| FR-D03 | Shell SHALL include Menu, ToolBar, header theme `ComboBox`, StatusBar, and a `TabControl` so specimens fit a 1280×860 window (min 960×640). |
| FR-D04 | Specimens live in `Views/` UserControls, one tab group each: Buttons, Data entry, Lists, PingIQ, DnsIQ, TraceIQ, Navigation, Layout, Console. |
| FR-D05 | PingIQ SHALL demonstrate `DataGrid.Standard` with status triggers, context menu, and a selected-row detail strip. |
| FR-D06 | DnsIQ SHALL demonstrate `DataGrid.Compact`. TraceIQ SHALL demonstrate `Border.Card` hops plus `DataGrid.Card`. |
| FR-D07 | Theme switch from the header combo **and** **View → Theme** SHALL call `SwitchTheme` and repaint DynamicResource-bound visuals without restart. |
| FR-D08 | Demo is a gallery, not a production instrument. Mock data is sufficient. |
| FR-D09 | Adding a catalog style key SHALL add a specimen on the matching tab in the same change. |

---

## 6. Non-functional requirements

| ID | Requirement |
|----|-------------|
| NFR-01 | Palette swap SHALL NOT recreate windows or require `InitializeComponent` again. Existing trees update via `{DynamicResource}`. |
| NFR-02 | Lookup order: last merged dictionary wins. Palette is merged after Metrics and Controls so token values override. |
| NFR-03 | Theme load of an incomplete palette SHALL fail before the previous palette is removed. |
| NFR-04 | Contrast: body text on `Surface.Window` / `Surface.Card` MUST remain readable; primary CTA text on `Accent.Primary` MUST remain readable (see FR-P02). |
| NFR-05 | Focus visuals remain visible on both light and dark palettes (`Stroke.Focus`). |
| NFR-06 | Core public API is small (`IThemeManager` + definitions + validator). No hidden static `Application.Current` writes except through `Initialize`/`SwitchTheme`/`Unload`. |
| NFR-07 | Deterministic builds (`Directory.Build.props`). |

---

## 7. Token contract (normative)

Required color keys (each has a matching `Vestigium.Brushes.*` unless noted):

**Surface** — Window, Layer, Card, Sunken, Chrome, Sidebar, StatusBar, Header, Hover, Disabled  
**Control** — Fill, FillHover, FillPressed, FillDisabled, FillInput  
**Text** — Primary, Secondary, Tertiary, Disabled, Inverse, Heading, OnSubtle, Inactive, Code, Link  
**Accent** — Primary, Hover, Pressed, Subtle, Softer  
**Selection** — Fill, FillInactive, Text, Highlight, CellFocus  
**Stroke** — Default, Subtle, Strong, Focus, Input, Disabled  
**Status** — Success, SuccessHover, SuccessPressed, SuccessFill, Warning, WarningHover, WarningPressed, WarningFill, Error, ErrorHover, ErrorPressed, ErrorFill, Info, InfoFill, Neutral, NeutralFill  
**Grid** — Line, AlternateRow, Header, RowHover  
**Scroll** — Thumb, ThumbHover, ThumbDrag, Track  
**Shadow**  
**Terminal** — Background, Foreground, Border, BackgroundDisabled, Comment, Keyword, String, Number, Error, Cursor  
**Series** — 1 … 6  
**Overlay** — `Vestigium.Brushes.Overlay.Scrim` only (no color key)

`ThemeResourceKeys.ContractVersion` is `"1.0"`.

---

## 8. Constraints and rules of use

1. Hosts call `Initialize` in `OnStartup` **before** `base.OnStartup` / `StartupUri` materializes `MainWindow`.
2. `{StaticResource}` on style keys; `{DynamicResource}` on colors and brushes.
3. Never `Clear()` `Application.Resources` to change themes.
4. Do not put palette project references in `Vestigium.Themes.csproj`.
5. `Color` properties (`DropShadowEffect.Color`, `GradientStop.Color`) bind `Vestigium.Colors.*`. `Background` / `Foreground` / `Fill` / `Stroke` / `BorderBrush` bind `Vestigium.Brushes.*`.
6. XAML attribute values that contain C# quotes (code specimens) MUST use opposite quote delimiters or `"`.

---

## 9. Acceptance criteria (v1.0)

The release is accepted when all of the following hold on Windows under Visual Studio 2026:

1. `Vestigium.Themes.slnx` restores and builds Debug + Release for `net10.0-windows`.
2. Demo starts with Light Blue applied; no `XamlParseException` on catalog dictionaries.
3. Switching all eight palettes from the header combo updates chrome, grids, and terminal samples without restart.
4. `ThemeContractValidator` reports **zero** missing keys for every shipped `Theme.xaml`.
5. Unregistering is not required; `Unload()` returns stock WPF and `Button.Primary` no longer resolves.
6. StandardWPF still resolves `Button.Primary` (it is a palette).
7. PingIQ tab DataGrid shows per-row status colors via triggers.
8. Console tab compiles (quoted C# specimens are valid XAML) and uses `Vestigium.Brushes.Terminal.*`.
9. Slider thumb drag glow uses `Vestigium.Colors.Accent.Primary` (not a brush) on `DropShadowEffect.Color`.
10. Documentation at `_Documentation/DevelopersGuide_v1.0.md` and this file describe the same contract version.

---

## 10. Traceability

| Requirement group | Primary implementation |
|-------------------|------------------------|
| FR-A\* | `src/Vestigium.Themes`, `src/Vestigium.Themes.Controls`, `src/Vestigium.Themes.*` |
| FR-M\* | `ThemeManager.cs`, `IThemeManager.cs` |
| FR-C\* | `ThemeResourceKeys.cs`, `ThemeContractValidator.cs`, each `Themes/Theme.xaml` |
| FR-S\* | `src/Vestigium.Themes.Controls/Themes/**` |
| FR-P\* / TH-\* | Palette projects |
| FR-D\* | `src/Vestigium.Themes.Demo` |
| PL-\* | `Directory.Build.props`, `*.csproj`, `Vestigium.Themes.slnx` |

---

## 11. Document control

| Field | Value |
|-------|--------|
| Document | Requirements_v1.0.md |
| Location | `_Documentation/` |
| Contract | 1.0 |
| Breaking change policy | New required keys or renamed keys ⇒ bump `ThemeResourceKeys.ContractVersion` and this document |

*Vestigium.Themes — explicit, semantic, native WPF. Requirements 1.0.*
