namespace Vestigium.Themes.Demo;

public sealed class PingRow
{
    public required string Host { get; init; }
    public required string Ip { get; init; }
    public required string Min { get; init; }
    public required string Avg { get; init; }
    public required string Max { get; init; }
    public required string Loss { get; init; }
    public required string Sent { get; init; }
    public required string Status { get; init; }
}

public sealed class DnsRow
{
    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string Data { get; init; }
    public required string Ttl { get; init; }
}

public sealed class TraceHop
{
    public required int Hop { get; init; }
    public required string Host { get; init; }
    public required string Address { get; init; }
    public required string Rtt { get; init; }
    public required string Status { get; init; }
}

public sealed class LogLine
{
    public required string Stamp { get; init; }
    public required string Message { get; init; }
    public string Level { get; init; } = "info";
    public string Line => $"{Stamp}  {Message}";
}
