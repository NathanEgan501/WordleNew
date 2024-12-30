using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class LightTheme : ResourceDictionary
    {

        public LightTheme()
        {
            this.Add("BackgroundColor", Colors.White);
            this.Add("TextColor", Colors.Black);
            this.Add("MediumFontSize", 18.0);
        }
    }
}