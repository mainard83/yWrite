using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace yWrite_UWP
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var window = new Window();
            window.Content = new MainPage();
            window.Title = "yWrite";
            window.AppWindow.SetIcon("Assets/StoreLogo.png");
            window.Activate();
        }
    }
}