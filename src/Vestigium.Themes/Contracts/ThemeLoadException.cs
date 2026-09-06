namespace Vestigium.Themes;

public sealed class ThemeLoadException : Exception
{
    public ThemeLoadException(string message) : base(message) { }
    public ThemeLoadException(string message, Exception inner) : base(message, inner) { }

    public IReadOnlyList<string> MissingKeys { get; init; } = [];
}
