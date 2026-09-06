namespace Vestigium.Themes;

/// <summary>
/// Swaps the active palette ResourceDictionary in Application.MergedDictionaries.
/// Core metrics + control styles are merged once and never replaced.
/// </summary>
public sealed class ThemeManager : IThemeManager
{
    public const string ControlsPackUri =
        "pack://application:,,,/Vestigium.Themes.Controls;component/Themes/Controls.xaml";

    public const string MetricsPackUri =
        "pack://application:,,,/Vestigium.Themes;component/Themes/Metrics.xaml";

    private readonly List<ThemeDefinition> _themes = [];
    private Application? _application;
    private bool _coreLoaded;

    public IReadOnlyList<ThemeDefinition> AvailableThemes => _themes;
    public ThemeDefinition? Current { get; private set; }
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public void Register(ThemeDefinition theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        if (_themes.Exists(t => t.Id.Equals(theme.Id, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Theme '{theme.Id}' is already registered.");
        }

        _themes.Add(theme);
    }

    public void Initialize(Application application, string? themeId = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        _application = application;
        EnsureCore(application.Resources.MergedDictionaries);

        if (_themes.Count == 0)
        {
            throw new ThemeLoadException("No themes registered. Call Register() before Initialize().");
        }

        var id = string.IsNullOrWhiteSpace(themeId) ? _themes[0].Id : themeId;
        SwitchTheme(id);
    }

    public void SwitchTheme(string themeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeId);
        var app = RequireApplication();

        var theme = _themes.Find(t => t.Id.Equals(themeId, StringComparison.OrdinalIgnoreCase))
            ?? throw new ThemeLoadException($"Unknown theme '{themeId}'.");

        ResourceDictionary palette;
        try
        {
            palette = new ResourceDictionary { Source = theme.ResourceUri };
        }
        catch (Exception ex)
        {
            throw new ThemeLoadException($"Failed to load '{theme.Id}' from {theme.ResourceUri}.", ex);
        }

        ThemeContractValidator.Validate(palette);

        var merged = app.Resources.MergedDictionaries;
        EnsureCore(merged);
        RemovePalettes(merged);
        merged.Add(palette);

        var previous = Current;
        Current = theme;
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(previous, theme));
    }

    /// <summary>
    /// Removes Vestigium palettes and control dictionaries so stock WPF chrome returns.
    /// Style keys such as Button.Primary will no longer resolve.
    /// </summary>
    public void Unload()
    {
        var app = RequireApplication();
        var merged = app.Resources.MergedDictionaries;
        RemovePalettes(merged);
        RemoveBySource(merged, ControlsPackUri);
        RemoveBySource(merged, MetricsPackUri);
        _coreLoaded = false;
        var previous = Current;
        Current = null;
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(previous, null));
    }

    private Application RequireApplication()
        => _application ?? Application.Current
           ?? throw new InvalidOperationException("No WPF Application is running. Call Initialize() first.");

    private void EnsureCore(IList<ResourceDictionary> merged)
    {
        if (_coreLoaded) return;

        if (!HasSource(merged, MetricsPackUri))
        {
            merged.Insert(0, new ResourceDictionary { Source = new Uri(MetricsPackUri, UriKind.Absolute) });
        }

        if (!HasSource(merged, ControlsPackUri))
        {
            merged.Add(new ResourceDictionary { Source = new Uri(ControlsPackUri, UriKind.Absolute) });
        }

        _coreLoaded = true;
    }

    private static void RemovePalettes(IList<ResourceDictionary> merged)
    {
        for (var i = merged.Count - 1; i >= 0; i--)
        {
            if (merged[i].Contains(ThemeResourceKeys.ThemeId))
            {
                merged.RemoveAt(i);
            }
        }
    }

    private static void RemoveBySource(IList<ResourceDictionary> merged, string uri)
    {
        for (var i = merged.Count - 1; i >= 0; i--)
        {
            var source = merged[i].Source?.ToString();
            if (source is not null && source.Equals(uri, StringComparison.OrdinalIgnoreCase))
            {
                merged.RemoveAt(i);
            }
        }
    }

    private static bool HasSource(IList<ResourceDictionary> merged, string uri)
        => merged.Any(d =>
            d.Source is not null &&
            d.Source.ToString().Equals(uri, StringComparison.OrdinalIgnoreCase));
}
