using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class DarkTheme : ResourceDictionary
    {

        public DarkTheme()
        {
            this.Add("BackgroundColor", Colors.Black);
            this.Add("TextColor", Colors.White);
            this.Add("MediumFontSize", 18.0);
        }
    }
}