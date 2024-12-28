using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent(); 
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            var app = (App)Application.Current;
            app.ToggleTheme(); 
        }

        private void OnFontSizeChanged(object sender, ValueChangedEventArgs e)
        {
            double newSize = e.NewValue;
            Application.Current.Resources["MediumFontSize"] = newSize; 
        }
    }
}