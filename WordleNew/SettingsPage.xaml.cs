using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            if (Application.Current.Resources.ContainsKey("MediumFontSize"))
            {
                FontSizeSlider.Value = (double)Application.Current.Resources["MediumFontSize"];
            }
        }

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            var app = (App)Application.Current;
            app.ToggleTheme();
        }

        private void OnFontSizeChanged(object sender, ValueChangedEventArgs e)
        {
            double newSize = e.NewValue;
            Console.WriteLine($"Font size changed to: {newSize}");
            Application.Current.Resources["MediumFontSize"] = newSize;
        }
    }
}