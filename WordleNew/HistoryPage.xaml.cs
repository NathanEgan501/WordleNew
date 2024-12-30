using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace WordleNew
{
    public partial class HistoryPage : ContentPage
    {
        private const string FileName = "gameHistory.json";
        private List<GameAttempt> gameAttempts;

        public HistoryPage()
        {
            InitializeComponent();
            LoadGameHistory();
            HistoryListView.ItemsSource = null;
            HistoryListView.ItemsSource = gameAttempts; 
        }

        private void LoadGameHistory()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameHistory.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                gameAttempts = JsonSerializer.Deserialize<List<GameAttempt>>(json) ?? new List<GameAttempt>();
            }
            else
            {
                gameAttempts = new List<GameAttempt>();
            }
        }
    }
}