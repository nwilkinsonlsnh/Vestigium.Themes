using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vestigium.Themes;

namespace Vestigium.Themes.Demo;

public partial class MainViewModel : ObservableObject
{
    private readonly IThemeManager _themes;

    public MainViewModel(IThemeManager themes)
    {
        _themes = themes;
        Themes = themes.AvailableThemes;
        SelectedTheme = themes.Current ?? themes.AvailableThemes[0];
        _themes.ThemeChanged += (_, e) => SelectedTheme = e.Current;
    }

    public IReadOnlyList<ThemeDefinition> Themes { get; }

    [ObservableProperty]
    private ThemeDefinition? selectedTheme;

    [ObservableProperty]
    private string host = "8.8.8.8";

    partial void OnSelectedThemeChanged(ThemeDefinition? value)
    {
        if (value is null) return;
        if (_themes.Current?.Id == value.Id) return;
        _themes.SwitchTheme(value.Id);
    }

    [RelayCommand]
    private void Ping()
    {
        /* gallery only */
    }
}
