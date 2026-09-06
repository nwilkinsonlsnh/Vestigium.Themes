# Vestigium.Themes — Developers Guide

Verbose reference for hosts that consume the library. For clone-and-run steps see [`README.md`](../README.md).

---

## 1. What this library is

Vestigium.Themes is a **modular WPF theming stack**:

- **One control language** — explicit style keys such as `Button.Primary`, `DataGrid.Standard`, `TabItem.Standard`.
- **Many palettes** — XAML-only packages that supply the same `Vestigium.Colors.*` / `Vestigium.Brushes.*` keys with different values.
- **Runtime swap** — `ThemeManager` replaces only the palette `ResourceDictionary`. Metrics and control styles stay merged. `Application.Resources` is never cleared.

The host **registers** the theme packages it references. Core never takes a project reference to a palette DLL.

**Target:** .NET 10 / `net10.0-windows` / WPF / Visual Studio 2026.

---

## 2. Solution map

```
Vestigium.Themes.slnx
├── src/Vestigium.Themes                 contracts, ThemeManager, Metrics.xaml, Converters.xaml
├── src/Vestigium.Themes.Controls        explicit styles (DynamicResource → tokens)
├── src/Vestigium.Themes.LightBlue       palette
├── src/Vestigium.Themes.DarkMode
├── src/Vestigium.Themes.Terminal
├── src/Vestigium.Themes.SolarizedDark
├── src/Vestigium.Themes.StandardWPF     SystemColors-like hues, same keys
├── src/Vestigium.Themes.Monokai
├── src/Vestigium.Themes.Sublime         Mariana (Sublime Text default)
├── src/Vestigium.Themes.Dracula
├── src/Vestigium.Themes.Demo            WinExe gallery
│   ├── MainWindow.xaml                 Menu + ToolBar + TabControl shell
│   ├── MainViewModel.cs                IThemeManager + mock Ping/DNS/trace data
│   └── Views/                          one UserControl per tab
└── _Documents/                         this guide
```

Theme packages are **XAML-only** (`EnableDefaultCompileItems=false`). They compile as WPF class libraries so pack URIs resolve.

---

## 3. Design principles

| Principle | What it means |
|-----------|----------------|
| **Explicit styles** | No implicit `TargetType` defaults. Every control gets `Style="{StaticResource …}"`. |
| **Static on keys, Dynamic on color** | `StaticResource` for `Button.Primary`. `{DynamicResource Vestigium.Brushes.*}` inside templates so a palette swap repaints the tree. |
| **Semantic variants** | Prefer intent (`Primary`, `Error`, `Success`) over hex in views. |
| **Identical contract** | Every `Theme.xaml` supplies the keys in `ThemeResourceKeys`. `ThemeContractValidator` fails the load if a key is missing. |
| **Reserved palette slot** | `ThemeManager` finds dictionaries that contain `Vestigium.Theme.Id` and replaces that slot only. |
| **Accessible focus** | Shared dashed focus visuals; keep keyboard focus visible. |

---

## 4. Host integration

### 4.1 Project references

The executable references:

- `Vestigium.Themes`
- `Vestigium.Themes.Controls`
- **Only the palettes it ships** (e.g. LightBlue + DarkMode + Dracula)

Unused palettes are not loaded.

### 4.2 Register, initialize, switch

Call this **before** the first window is parsed so `{StaticResource Button.Primary}` resolves. `OnStartup` before `base.OnStartup` (and before `StartupUri` materializes `MainWindow`) is the right place.

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    var manager = new ThemeManager();
    manager.Register(ThemeDefinition.FromPack(
        "LightBlue", "Light Blue", "Vestigium.Themes.LightBlue", isDark: false,
        "Default Vestigium diagnostic chrome."));
    manager.Register(ThemeDefinition.FromPack(
        "Dracula", "Dracula", "Vestigium.Themes.Dracula", isDark: true));
    manager.Initialize(this, "LightBlue");

    // optional: put IThemeManager in your DI container
    base.OnStartup(e);
}
```

Later:

```csharp
manager.SwitchTheme("Dracula");
```

`ThemeDefinition.FromPack` builds:

```
pack://application:,,,/{assemblyName};component/Themes/Theme.xaml
```

### 4.3 What Initialize actually merges

In order:

1. `Vestigium.Themes;component/Themes/Metrics.xaml` (fonts, radii, spacing)
2. `Vestigium.Themes.Controls;component/Themes/Controls.xaml` (style catalog)
3. The active palette `Theme.xaml`

`SwitchTheme` validates the new palette, removes any merged dictionary that contains `Vestigium.Theme.Id`, then appends the new one. Lookup walks merged dictionaries last-to-first, so the palette wins for brush keys.

### 4.4 Views

```xml
<Button Style="{StaticResource Button.Primary}" Content="Save" />

<Border Background="{DynamicResource Vestigium.Brushes.Surface.Card}"
        BorderBrush="{DynamicResource Vestigium.Brushes.Stroke.Subtle}">
    <TextBlock Foreground="{DynamicResource Vestigium.Brushes.Text.Primary}"
               Text="Probe complete" />
</Border>
```

Do **not** write `#0078D7` in a view. That color belongs to Light Blue only; Dark Mode, Monokai, and Dracula will look wrong.

---

## 5. Standard WPF vs Unload

| Mechanism | Style keys exist? | When to use |
|-----------|-------------------|-------------|
| `Vestigium.Themes.StandardWPF` | Yes. Hues approximate Windows chrome / `SystemColors`. | User wants a “platform default” look **inside** the Vestigium language. |
| `ThemeManager.Unload()` | No. `Button.Primary` will not resolve. | Host is leaving Vestigium chrome entirely. |

StandardWPF is a palette, not “no theming.”

---

## 6. Resource contract (v1.0)

`ThemeResourceKeys` is the locked list. Every palette `Theme.xaml` must provide:

**Metadata**

- `Vestigium.Theme.Id`
- `Vestigium.Theme.DisplayName`
- `Vestigium.Theme.Base` (`Light` / `Dark`)
- `Vestigium.Theme.IsDark`
- `Vestigium.Theme.Version`
- `Vestigium.Theme.Showcase` (a `Color`)

`Vestigium.Theme.Description` is optional.

**Colors + matching brushes** for surfaces, text, accent, selection, stroke, status, grid, scroll, terminal, and series. Plus `Vestigium.Brushes.Overlay.Scrim`.

`ThemeContractValidator.Validate(ResourceDictionary)` throws `ThemeLoadException` with `MissingKeys` populated.

Brush keys are `SolidColorBrush` objects. Color keys are `Color`. Styles bind to **brushes**.

`#FFFFFF` is dual-use in source catalogs (inverse text vs card fill). Conversion is property-aware:

- `Foreground` / `CaretBrush` / `Stroke` / `Fill` → `Vestigium.Brushes.Text.Inverse`
- `Background` / `BorderBrush` → `Vestigium.Brushes.Surface.Card`

---

## 7. Pack URIs

```
pack://application:,,,/Vestigium.Themes.{Id};component/Themes/Theme.xaml
pack://application:,,,/Vestigium.Themes.Controls;component/Themes/Controls.xaml
pack://application:,,,/Vestigium.Themes;component/Themes/Metrics.xaml
```

`Controls.xaml` merges `Converters.xaml` from core, then every `*.Styles.xaml`. **ScrollBar is merged before ScrollViewer.**

---

## 8. Adding a palette

1. Copy an existing package (e.g. `Vestigium.Themes.Dracula`).
2. Rename the assembly / root namespace / `Vestigium.Theme.Id`.
3. Retint **every** `Vestigium.Colors.*` value. Brushes already `{StaticResource}` the color keys.
4. Keep the key set identical. Run the demo, `SwitchTheme` to the new id, confirm `ThemeContractValidator` does not throw.
5. Register it from the host. Core still does not reference the new DLL.

Suggested mapping when you start from an editor theme:

| Editor token | Vestigium key |
|--------------|----------------|
| Background | `Surface.Window` |
| Current line / highlight | `Surface.Hover` / `Selection.Fill` |
| Foreground | `Text.Primary` |
| Comment | `Text.Disabled` + `Terminal.Comment` |
| Keyword / accent | `Accent.Primary` |
| String | `Terminal.String` |
| Number | `Terminal.Number` |
| Error | `Status.Error` |
| Warning | `Status.Warning` |
| Green / insert | `Status.Success` |

Primary-button text uses `Text.Inverse`. If the accent is light (Dracula purple, Terminal green), inverse should be a **dark** canvas color so glyphs stay readable. If the accent is saturated pink/blue (Monokai, Sublime), inverse can be light.

---

## 9. Layout patterns

### Shell

```xml
<DockPanel Style="{StaticResource DockPanel.Standard}">
    <Border Style="{StaticResource Border.AppHeader}" DockPanel.Dock="Top">
        <!-- title / theme combo -->
    </Border>
    <StatusBar Style="{StaticResource StatusBar.Standard}" DockPanel.Dock="Bottom">
        <StatusBarItem Style="{StaticResource StatusBarItem.Standard}">
            <TextBlock Text="Ready" Style="{StaticResource TextBlock.Caption}" />
        </StatusBarItem>
    </StatusBar>
    <Border Style="{StaticResource Border.AppSidebar}" DockPanel.Dock="Left" Width="240">
        <TreeView Style="{StaticResource TreeView.Flush}">
            <!-- accordion items -->
        </TreeView>
    </Border>
    <TabControl Style="{StaticResource TabControl.Standard}">
        <!-- modules -->
    </TabControl>
</DockPanel>
```

### Cards and forms

```xml
<Border Style="{StaticResource Border.Card}">
    <StackPanel Style="{StaticResource StackPanel.FormGroup}">
        <Label Style="{StaticResource Label.Standard}" Content="_Host:" Target="{Binding ElementName=HostBox}" />
        <TextBox x:Name="HostBox" Style="{StaticResource TextBox.Standard}" />
    </StackPanel>
</Border>

<GroupBox Style="{StaticResource GroupBox.Standard}" Header="Connection">
    <!-- fields -->
</GroupBox>
```

`StackPanel.FormGroup` does **not** auto-gap children. Set margins on the children (or accept the catalog defaults).

### DataGrid (PingIQ-style)

```xml
<DataGrid Style="{StaticResource DataGrid.Standard}"
          ItemsSource="{Binding PingTargets}"
          SelectedItem="{Binding SelectedPing}"
          IsReadOnly="True"
          HeadersVisibility="Column">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Host" Binding="{Binding Host}" Width="*" />
        <DataGridTextColumn Header="Avg" Binding="{Binding Avg}" Width="80" />
        <DataGridTemplateColumn Header="Status" Width="140">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
                        <Ellipse Style="{StaticResource Ellipse.Success}" Width="10" Height="10" Margin="0,0,8,0" />
                        <TextBlock Text="{Binding Status}" Style="{StaticResource TextBlock.Standard}" />
                    </StackPanel>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>
```

`DataGrid.Standard` already applies header / row / cell styles, full-row selection, and horizontal gridlines. Use `DataGrid.Compact` in dense DNS-style lists.

Bind status **fills** with `DataTrigger` → `Vestigium.Brushes.Status.Success|Warning|Error` so they follow the palette.

### TreeView accordion (≤ 3 levels)

```xml
<TreeView Style="{StaticResource TreeView.Flush}">
    <TreeViewItem Header="Instruments" Style="{StaticResource TreeViewItem.AccordionParent}" IsExpanded="True">
        <TreeViewItem Header="Hamilton" Style="{StaticResource TreeViewItem.AccordionGroup}" IsExpanded="True">
            <TreeViewItem Header="Microlab Prep" Style="{StaticResource TreeViewItem.AccordionChild}" />
        </TreeViewItem>
    </TreeViewItem>
</TreeView>
```

| Level | Style |
|-------|--------|
| 0 | `TreeViewItem.AccordionParent` |
| 1 | `TreeViewItem.AccordionGroup` |
| 2 | `TreeViewItem.AccordionChild` |

Deeper than three levels: `TreeViewItem.Standard`.

### Buttons by intent

| Style | Use when |
|-------|----------|
| `Button.Primary` | Main CTA (Save, Continue, Ping) |
| `Button.Secondary` | Cancel / outline |
| `Button.Success` | Start / resume |
| `Button.Warning` | Pause / caution |
| `Button.Danger` | Delete / stop |
| `Button.Utility` | Ghost / diagnostic |

### Validation

Prefer Error variants instead of ad-hoc red borders:

```xml
<TextBox Style="{StaticResource TextBox.Error}" Text="{Binding Path}" />
<ComboBox Style="{StaticResource ComboBox.Error}" />
<DatePicker Style="{StaticResource DatePicker.Error}" />
<CheckBox Style="{StaticResource CheckBox.Error}" Content="Required" />
```

### Console / terminal

```xml
<RichTextBox Style="{StaticResource RichTextBox.ConsoleLog}" IsReadOnly="True" />
```

That style is bound to `Vestigium.Brushes.Terminal.*`, not a hardcoded `#1E1E1E`.

---

## 10. The Demo

`Vestigium.Themes.Demo` is the visual contract test. It is a small MVVM host (`CommunityToolkit.Mvvm` + Microsoft.Extensions.DependencyInjection), not a production instrument — but the chrome is the same language PingIQ / DnsIQ / TraceIQ will use.

### Shell

- **Menu.Standard** — File (Ping / Stop / Export / Exit), View → Theme (all eight palettes), Help.
- **ToolBarTray.Standard** + **ToolBar.Standard** — Ping / Stop / Export with explicit button styles.
- **Border.AppHeader** — title plus combo bound to `IThemeManager.AvailableThemes`. Changing `SelectedTheme` calls `SwitchTheme`.
- **StatusBar.Standard** — current theme, selected ping host, status text, contract version.
- **TabControl.Standard** — one tab per specimen group so the window stays usable at 1280×860.

### Tabs (`Views/`)

| Tab | UserControl | Specimens |
|-----|-------------|-----------|
| Buttons | `ButtonsTab` | Six button intents, disabled, Ellipse status dots, ProgressBar.*, Slider.Filled / Stepped / Standard |
| Data entry | `DataEntryTab` | GroupBox Probe/Query, TextBox, PasswordBox, ComboBox (standard / editable / error), DatePicker, CheckBox / ToggleSwitch / Card / Error, RadioButton.Standard / Card / TextLink |
| Lists | `ListsTab` | ListBox.Standard, ListBox.Card, ListView.Standard + GridView bound to the same `PingTargets` |
| PingIQ | `PingIqTab` | **DataGrid.Standard** — Host, IP, Min/Avg/Max, Loss, Sent, status `DataTrigger` fills. Context menu. Detail strip for `SelectedPing`. |
| DnsIQ | `DnsIqTab` | Query bar + **DataGrid.Compact** with type chips (`Status.InfoFill`) |
| TraceIQ | `TraceIqTab` | `Border.Card` hop tiles + **DataGrid.Card** |
| Navigation | `NavigationTab` | TreeView.Flush accordion, GridSplitter, nested TabControl.Standard / Pill / Vertical, RadioButton.Pill / HorizontalTab / VerticalNav, Expander.Card |
| Layout | `LayoutTab` | Calendar.Standard, GridSplitter.Bar, Border.Accent / Error / Card / CardShadow / Panel, ContentControl.Card, typography, separators, rectangles |
| Console | `ConsoleTab` | Bound `ItemsControl` on Terminal.* brushes (level colors), RichTextBox.ConsoleLog, RichTextBox.CodeViewer |

When you add a style key, add a specimen to the matching tab. Do not dump every control onto one surface — WPF form real estate is the reason the demo is tabbed.

### DataGrid notes (PingIQ)

Status fills **must** be `DataTrigger` → `Vestigium.Brushes.Status.Success|Warning|Error`. A static `Ellipse.Success` on every row would lie about timeouts.

```xml
<DataTrigger Binding="{Binding Status}" Value="Timeout">
    <Setter Property="Fill" Value="{DynamicResource Vestigium.Brushes.Status.Error}" />
</DataTrigger>
```

`DataGrid.Standard` already applies header / row / cell styles, full-row selection, and horizontal gridlines. Use Compact for DNS, Card for quieter hop lists.


---

## 11. Style catalog (consumer keys)

Skip `*.Base`, focus visuals, and private thumbs unless you are extending the catalog.

### Data entry

`Button.Primary` `Button.Secondary` `Button.Success` `Button.Warning` `Button.Danger` `Button.Utility`  
`Calendar.Standard` `Calendar.Compact`  
`CheckBox.Standard` `CheckBox.Error` `CheckBox.ToggleSwitch` `CheckBox.Card`  
`ComboBox.Standard` `ComboBox.Error` `ComboBox.InlineEdit` `ComboBox.Editable` `ComboBoxItem.Standard`  
`DatePicker.Standard` `DatePicker.Error` `DatePicker.InlineEdit` `DatePicker.ReadOnly`  
`PasswordBox.Standard` `PasswordBox.Error`  
`RadioButton.Standard` `RadioButton.HorizontalTab` `RadioButton.Pill` `RadioButton.TextLink` `RadioButton.Card` `RadioButton.VerticalNav`  
`RichTextBox.Standard` `RichTextBox.ConsoleLog` `RichTextBox.CodeViewer` `RichTextBox.ReadOnly`  
`Slider.Standard` `Slider.Filled` `Slider.Stepped`  
`TextBox.Standard` `TextBox.Error` `TextBox.ReadOnly` `TextBox.InlineEdit`

### Data presentation

`DataGrid.Standard` `DataGrid.Compact` `DataGrid.Card`  
`Expander.Standard` `Expander.Card` `Expander.Flush`  
`ListBox.Standard` `ListBox.Card` `ListBox.Flush` `ListBoxItem.Standard` `ListBoxItem.Card`  
`ListView.Standard` `ListView.Flush`  
`TabControl.Standard` `TabControl.Pill` `TabControl.Vertical` `TabItem.Standard` `TabItem.Pill` `TabItem.Vertical`  
`TreeView.Standard` `TreeView.Flush` `TreeViewItem.Standard` `TreeViewItem.AccordionParent` `TreeViewItem.AccordionGroup` `TreeViewItem.AccordionChild`

### Layout

`Border.Standard` `Border.Card` `Border.CardShadow` `Border.Panel` `Border.Accent` `Border.Error` `Border.AppHeader` `Border.AppSidebar` `Border.StatusBar`  
`GroupBox.Standard` `GroupBox.Card` `GroupBox.Flush`  
`DockPanel.Standard` `Grid.Standard` `GridSplitter.Standard` `GridSplitter.Bar`  
`StackPanel.FormGroup` `StackPanel.Toolbar`  
`WrapPanel.Standard` `WrapPanel.CardGrid` `WrapPanel.Toolbar`  
`Canvas.Standard` `Canvas.Grid` `Canvas.Transparent`  
`ContentControl.Standard` `ContentControl.Card` `ContentControl.Panel`  
`Viewbox.Standard` `Viewbox.Fill` `Viewbox.UniformToFill`

### Menus & chrome

`Menu.Standard` `MenuItem.Standard` `ContextMenu.Standard` `Separator.Menu`  
`StatusBar.Standard` `StatusBarItem.Standard` `Separator.StatusBar`  
`ToolBar.Standard` `ToolBar.Card` `ToolBarTray.Standard` `ToolBarTray.Vertical` `ToolBarTray.Locked`

### Navigation & media

`ProgressBar.Standard` `ProgressBar.Thin` `ProgressBar.Success` `ProgressBar.Error`  
`ScrollViewer.Standard` `ScrollViewer.Horizontal` `ScrollViewer.Both` `ScrollViewer.Minimal` `ScrollViewer.Both.Minimal`  
`Separator.Standard` `Separator.Vertical` `Separator.Heavy`  
`Frame.Standard` `Frame.Bordered` `DocumentViewer.Standard` `DocumentViewer.Borderless`  
`Ellipse.Standard` `Ellipse.Accent` `Ellipse.Success` `Ellipse.Warning` `Ellipse.Error`  
`Image.Standard` `Image.Cover` `Image.Fill` `Image.None`  
`Rectangle.Standard` `Rectangle.Divider` `Rectangle.Accent` `Rectangle.Rounded`

### Typography

`Label.Standard` `Label.Small` `Label.Bold`  
`TextBlock.Standard` `TextBlock.Heading` `TextBlock.Subheader` `TextBlock.Caption` `TextBlock.Error`

---

## 12. Adding a control style

1. Create `src/Vestigium.Themes.Controls/Themes/{Category}/{Control}.Styles.xaml`.
2. Header comment listing keys and usage. No implicit `TargetType` styles.
3. Bind paints with `{DynamicResource Vestigium.Brushes.*}`.
4. `BasedOn` a `*.Base` style; include hover, pressed, disabled, keyboard focus.
5. Register the file in `Themes/Controls.xaml` (ScrollBar before ScrollViewer).
6. Add a specimen on the Demo TabControl.
7. Do not duplicate keys across dictionaries (`Separator.Standard` lives in one file).

---

## 13. Troubleshooting

| Symptom | Likely cause |
|---------|----------------|
| Default Aero look | Missing `Style="{StaticResource …}"` |
| `Button.Primary` not found at parse | `Initialize` ran after the window was loaded |
| Theme load throws | Palette missing a contract key — read `ThemeLoadException.MissingKeys` |
| Swap does nothing | Host used `{StaticResource}` on a **color** instead of `{DynamicResource}` |
| Primary button text vanishes on dark palettes | `Text.Inverse` mapped to card fill; inverse must contrast with `Accent.Primary` |
| Duplicate key / last-writer-wins | Same `x:Key` in two style files |
| ScrollViewer unthemed | `ScrollBar.Styles.xaml` not merged first |
| Pack URI fail | Palette project not referenced, or `Theme.xaml` not a Page (SDK + `UseWPF` should include it) |
| XML comment parse error | `--` inside an XML comment |

---

## 14. Related types

| Type | Role |
|------|------|
| `IThemeManager` | Register / Initialize / SwitchTheme / Unload |
| `ThemeManager` | Default implementation |
| `ThemeDefinition` | Id, display name, pack URI, IsDark |
| `ThemeResourceKeys` | Locked contract |
| `ThemeContractValidator` | Fail-fast on missing keys |
| `ThemeLoadException` | Includes `MissingKeys` |
| `ThemeChangedEventArgs` | Previous + current |

---

*Vestigium.Themes — explicit, semantic, native WPF. Contract 1.0.*
