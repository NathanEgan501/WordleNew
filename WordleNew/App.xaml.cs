using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class App : Application
    {
        private bool isDarkMode = false; // Default to light mode

        public App()
        {
            InitializeComponent();
            // Set the initial theme
            ApplyTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        public void ToggleTheme()
        {
            isDarkMode = !isDarkMode; // Toggle the mode
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            var mergedDictionaries = Resources.MergedDictionaries;
            mergedDictionaries.Clear();

            if (isDarkMode)
            {
                mergedDictionaries.Add(new DarkTheme());
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                mergedDictionaries.Add(new LightTheme());
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }
    }
}