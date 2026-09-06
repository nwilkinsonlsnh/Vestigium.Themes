using Microsoft.Extensions.DependencyInjection;

namespace Vestigium.Themes.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}
