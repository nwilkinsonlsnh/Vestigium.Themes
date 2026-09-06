namespace Vestigium.Themes;

public sealed class ThemeChangedEventArgs : EventArgs
{
    public ThemeChangedEventArgs(ThemeDefinition? previous, ThemeDefinition? current)
    {
        Previous = previous;
        Current = current;
    }

    public ThemeDefinition? Previous { get; }
    public ThemeDefinition? Current { get; }
}
