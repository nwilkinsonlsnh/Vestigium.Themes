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
            new() { Host = "dns.google", Ip = "8.8.8.8", Avg = "12 ms", Loss = "0%", Status = "Reachable" },
            new() { Host = "one.one.one.one", Ip = "1.1.1.1", Avg = "8 ms", Loss = "0%", Status = "Reachable" },
            new() { Host = "cloudflare-dns", Ip = "1.0.0.1", Avg = "9 ms", Loss = "0%", Status = "Reachable" },
            new() { Host = "edge.vestigium.local", Ip = "172.16.4.2", Avg = "48 ms", Loss = "4%", Status = "Degraded" },
            new() { Host = "core-sw-01.lab", Ip = "10.0.0.2", Avg = "2 ms", Loss = "0%", Status = "Reachable" },
            new() { Host = "mail.vestigium.local", Ip = "172.16.4.25", Avg = "31 ms", Loss = "1%", Status = "Reachable" },
            new() { Host = "gw-lab-01", Ip = "10.0.0.1", Avg = "—", Loss = "100%", Status = "Timeout" },
            new() { Host = "ntp.lab", Ip = "10.0.0.15", Avg = "1 ms", Loss = "0%", Status = "Reachable" },
        ];
        SelectedPing = PingTargets[1];

        DnsRecords =
        [
            new() { Type = "A", Name = "vestigium.local", Data = "172.16.4.20", Ttl = "300" },
            new() { Type = "AAAA", Name = "vestigium.local", Data = "2001:db8::14", Ttl = "300" },
            new() { Type = "MX", Name = "vestigium.local", Data = "10 mail.vestigium.local", Ttl = "3600" },
            new() { Type = "NS", Name = "vestigium.local", Data = "ns1.vestigium.local", Ttl = "86400" },
            new() { Type = "TXT", Name = "vestigium.local", Data = "v=spf1 include:_spf.vestigium.local", Ttl = "600" },
            new() { Type = "CNAME", Name = "www.vestigium.local", Data = "vestigium.local", Ttl = "300" },
        ];

        TraceHops =
        [
            new() { Hop = 1, Host = "gateway.lab", Rtt = "1.2 ms" },
            new() { Hop = 2, Host = "core-sw-01.lab", Rtt = "2.8 ms" },
            new() { Hop = 3, Host = "edge.isp.net", Rtt = "14.6 ms" },
            new() { Hop = 4, Host = "ix-ash.example.net", Rtt = "22.1 ms" },
            new() { Hop = 5, Host = "dns.google", Rtt = "24.0 ms" },
        ];

        ConsoleLines =
        [
            new() { Stamp = "14:02:11.041", Message = "ICMP echo  8.8.8.8     time=12.4 ms" },
            new() { Stamp = "14:02:12.044", Message = "ICMP echo  8.8.8.8     time=11.8 ms" },
            new() { Stamp = "14:02:13.040", Message = "ICMP echo  10.0.0.1    Destination host unreachable" },
            new() { Stamp = "14:02:14.051", Message = "ICMP echo  8.8.8.8     time=13.1 ms" },
            new() { Stamp = "14:02:15.048", Message = "ICMP echo  1.1.1.1     time=8.2 ms" },
        ];
    }

    public IReadOnlyList<ThemeDefinition> Themes { get; }

    public ObservableCollection<PingRow> PingTargets { get; }
    public ObservableCollection<DnsRow> DnsRecords { get; }
    public IReadOnlyList<TraceHop> TraceHops { get; }
    public IReadOnlyList<LogLine> ConsoleLines { get; }

    [ObservableProperty]
    private ThemeDefinition? selectedTheme;

    [ObservableProperty]
    private PingRow? selectedPing;

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

    partial void OnSelectedThemeChanged(ThemeDefinition? value)
    {
        if (value is null) return;
        if (_themes.Current?.Id == value.Id) return;
        _themes.SwitchTheme(value.Id);
    }

    [RelayCommand]
    private void Ping()
    {
        /* gallery only — host apps replace this with a real probe */
    }
}
