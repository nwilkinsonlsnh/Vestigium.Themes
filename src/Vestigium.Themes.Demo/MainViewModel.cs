using System.Collections.ObjectModel;
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

        PingTargets =
        [
            new() { Host = "dns.google", Ip = "8.8.8.8", Min = "9 ms", Avg = "12 ms", Max = "18 ms", Loss = "0%", Sent = "48", Status = "Reachable" },
            new() { Host = "one.one.one.one", Ip = "1.1.1.1", Min = "6 ms", Avg = "8 ms", Max = "14 ms", Loss = "0%", Sent = "48", Status = "Reachable" },
            new() { Host = "cloudflare-dns", Ip = "1.0.0.1", Min = "7 ms", Avg = "9 ms", Max = "15 ms", Loss = "0%", Sent = "48", Status = "Reachable" },
            new() { Host = "edge.vestigium.local", Ip = "172.16.4.2", Min = "22 ms", Avg = "48 ms", Max = "91 ms", Loss = "4%", Sent = "48", Status = "Degraded" },
            new() { Host = "core-sw-01.lab", Ip = "10.0.0.2", Min = "1 ms", Avg = "2 ms", Max = "3 ms", Loss = "0%", Sent = "48", Status = "Reachable" },
            new() { Host = "mail.vestigium.local", Ip = "172.16.4.25", Min = "18 ms", Avg = "31 ms", Max = "54 ms", Loss = "1%", Sent = "48", Status = "Reachable" },
            new() { Host = "gw-lab-01", Ip = "10.0.0.1", Min = "—", Avg = "—", Max = "—", Loss = "100%", Sent = "48", Status = "Timeout" },
            new() { Host = "ntp.lab", Ip = "10.0.0.15", Min = "1 ms", Avg = "1 ms", Max = "2 ms", Loss = "0%", Sent = "48", Status = "Reachable" },
            new() { Host = "files.vestigium.local", Ip = "172.16.4.40", Min = "11 ms", Avg = "19 ms", Max = "44 ms", Loss = "0%", Sent = "32", Status = "Reachable" },
            new() { Host = "vpn-gw.lab", Ip = "10.8.0.1", Min = "40 ms", Avg = "62 ms", Max = "110 ms", Loss = "8%", Sent = "32", Status = "Degraded" },
        ];
        SelectedPing = PingTargets[1];

        DnsRecords =
        [
            new() { Type = "A", Name = "vestigium.local", Data = "172.16.4.20", Ttl = "300" },
            new() { Type = "AAAA", Name = "vestigium.local", Data = "2001:db8::14", Ttl = "300" },
            new() { Type = "MX", Name = "vestigium.local", Data = "10 mail.vestigium.local", Ttl = "3600" },
            new() { Type = "NS", Name = "vestigium.local", Data = "ns1.vestigium.local", Ttl = "86400" },
            new() { Type = "NS", Name = "vestigium.local", Data = "ns2.vestigium.local", Ttl = "86400" },
            new() { Type = "TXT", Name = "vestigium.local", Data = "v=spf1 include:_spf.vestigium.local ~all", Ttl = "600" },
            new() { Type = "CNAME", Name = "www.vestigium.local", Data = "vestigium.local", Ttl = "300" },
            new() { Type = "SOA", Name = "vestigium.local", Data = "ns1.vestigium.local hostmaster", Ttl = "86400" },
        ];
        SelectedDns = DnsRecords[0];

        TraceHops =
        [
            new() { Hop = 1, Host = "gateway.lab", Address = "10.0.0.1", Rtt = "1.2 ms", Status = "Reachable" },
            new() { Hop = 2, Host = "core-sw-01.lab", Address = "10.0.0.2", Rtt = "2.8 ms", Status = "Reachable" },
            new() { Hop = 3, Host = "edge.isp.net", Address = "198.51.100.1", Rtt = "14.6 ms", Status = "Reachable" },
            new() { Hop = 4, Host = "ix-ash.example.net", Address = "203.0.113.8", Rtt = "22.1 ms", Status = "Reachable" },
            new() { Hop = 5, Host = "dns.google", Address = "8.8.8.8", Rtt = "24.0 ms", Status = "Reachable" },
        ];

        ConsoleLines =
        [
            new() { Stamp = "14:02:11.041", Message = "ICMP echo  8.8.8.8            time=12.4 ms", Level = "info" },
            new() { Stamp = "14:02:12.044", Message = "ICMP echo  8.8.8.8            time=11.8 ms", Level = "info" },
            new() { Stamp = "14:02:13.040", Message = "ICMP echo  10.0.0.1           Destination host unreachable", Level = "error" },
            new() { Stamp = "14:02:14.051", Message = "ICMP echo  8.8.8.8            time=13.1 ms", Level = "info" },
            new() { Stamp = "14:02:15.048", Message = "ICMP echo  1.1.1.1            time=8.2 ms", Level = "success" },
            new() { Stamp = "14:02:16.102", Message = "DNS query   vestigium.local A  172.16.4.20  ttl=300", Level = "info" },
            new() { Stamp = "14:02:16.440", Message = "trace hop 3  edge.isp.net      14.6 ms", Level = "info" },
            new() { Stamp = "14:02:17.011", Message = "warn         edge.vestigium.local  loss=4%", Level = "warn" },
        ];
    }

    public IReadOnlyList<ThemeDefinition> Themes { get; }

    public ObservableCollection<PingRow> PingTargets { get; }
    public ObservableCollection<DnsRow> DnsRecords { get; }
    public IReadOnlyList<TraceHop> TraceHops { get; }
    public IReadOnlyList<LogLine> ConsoleLines { get; }

    public IReadOnlyList<string> RecentQueries { get; } =
    [
        "8.8.8.8 — dns.google",
        "1.1.1.1 — one.one.one.one",
        "10.0.0.1 — gw-lab-01",
        "172.16.4.2 — edge.vestigium.local",
        "vestigium.local — IN A",
    ];

    public IReadOnlyList<string> Interfaces { get; } =
    [
        "Ethernet 2 — 10.0.0.24 / 24",
        "Wi-Fi — 172.16.4.88 / 24",
        "Loopback — 127.0.0.1",
        "Tailscale — 100.64.0.12",
    ];

    [ObservableProperty]
    private ThemeDefinition? selectedTheme;

    [ObservableProperty]
    private PingRow? selectedPing;

    [ObservableProperty]
    private DnsRow? selectedDns;

    [ObservableProperty]
    private string host = "8.8.8.8";

    [ObservableProperty]
    private string recordType = "A";

    [ObservableProperty]
    private bool icmp = true;

    [ObservableProperty]
    private bool resolvePtr = true;

    [ObservableProperty]
    private bool continuous;

    [ObservableProperty]
    private double timeoutMs = 1000;

    [ObservableProperty]
    private double payload = 32;

    [ObservableProperty]
    private double ttl = 128;

    [ObservableProperty]
    private string statusText = "Ready";

    partial void OnSelectedThemeChanged(ThemeDefinition? value)
    {
        if (value is null) return;
        if (_themes.Current?.Id == value.Id) return;
        _themes.SwitchTheme(value.Id);
        StatusText = $"Theme: {value.DisplayName}";
    }

    [RelayCommand]
    private void Ping()
    {
        StatusText = $"Pinging {Host}… (gallery stub)";
    }

    [RelayCommand]
    private void Stop()
    {
        StatusText = "Stopped";
    }

    [RelayCommand]
    private void Export()
    {
        StatusText = "Export is a gallery stub";
    }

    [RelayCommand]
    private void CopyHost()
    {
        if (SelectedPing is null) return;
        StatusText = $"Copied {SelectedPing.Host}";
    }

    [RelayCommand]
    private void ApplyTheme(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        _themes.SwitchTheme(id);
    }

    [RelayCommand]
    private void Exit() => Application.Current.Shutdown();
}
