using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage)); 
        }
    }
}