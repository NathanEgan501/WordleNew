using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace WordleNew
{
    public partial class MainPage : ContentPage
    {
        private string secretWord;
        private int attempts;
        private List<string> wordList = new List<string>();
        private int currentAttempt;
        private const string PlayerFileName = "player.txt";

        public MainPage()
        {
            InitializeComponent();
            LoadWordList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadPlayer();
        }

        private async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void LoadWordList()
        {
            try
            {
                using HttpClient client = new HttpClient();
                string wordData = await client.GetStringAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt");

                Console.WriteLine("Raw word data: " + wordData);

                if (string.IsNullOrWhiteSpace(wordData))
                {
                    Console.WriteLine("Warning: No data received from the word list URL.");
                    return;
                }

                wordList = new List<string>(wordData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));

                Console.WriteLine($"Word list size: {wordList.Count}");

                if (wordList.Count == 0)
                {
                    Console.WriteLine("Warning: Word list is empty after processing.");
                    return;
                }

                StartNewGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading word list: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async void LoadPlayer()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PlayerFileName);
            Console.WriteLine($"Checking for player file at: {filePath}");

            if (File.Exists(filePath))
            {
                string playerName = File.ReadAllText(filePath);
                MessageLabel.Text = $"Welcome back, {playerName}! Guess the word!";
            }
            else
            {
                Console.WriteLine("Player file does not exist. Prompting for name.");
                await PromptForPlayerName();
            }
        }

        private async Task PromptForPlayerName()
        {
            string playerName = await DisplayPromptAsync("New Player", "Please enter your name:", "OK", "Cancel", null, -1);

            if (!string.IsNullOrWhiteSpace(playerName))
            {
                SavePlayerName(playerName);
                MessageLabel.Text = $"Welcome, {playerName}! Guess the word!";
            }
            else
            {
                MessageLabel.Text = "Welcome! Guess the word!";
            }
        }

        private void SavePlayerName(string playerName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PlayerFileName);
            File.WriteAllText(filePath, playerName);
            Console.WriteLine($"Player name '{playerName}' saved to file.");
        }

        private void StartNewGame()
        {
            currentAttempt = 0;
            attempts = 6;
            secretWord = GetRandomWord();
            MessageLabel.Text = "Guess the word!";
            Debug.WriteLine("The secret word is: " + secretWord);
            ClearGuessGrid();
        }

        private string GetRandomWord()
        {
            if (wordList.Count == 0)
            {
                throw new InvalidOperationException("Word list is empty. Cannot select a random word.");
            }

            Random random = new Random();
            int index = random.Next(wordList.Count);
            string selectedWord = wordList[index].ToLower();
            Console.WriteLine("Secret word selected: " + selectedWord);
            return selectedWord;
        }

        private void ClearGuessGrid()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int index = i * 5 + j;
                    if (index < GuessGrid.Children.Count)
                    {
                        var label = (Label)GuessGrid.Children[index];
                        if (label != null)
                        {
                            label.Text = string.Empty;
                            label.TextColor = Colors.Black;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Index out of bounds: {index}");
                    }
                }
            }
        }

        private void OnSubmitGuessClicked(object sender, EventArgs e)
        {
            string guess = GuessEntry.Text?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(guess) || guess.Length != 5)
            {
                MessageLabel.Text = "Please enter a valid 5-letter word.";
                return;
            }

            if (currentAttempt >= attempts)
            {
                MessageLabel.Text = "No more attempts left! The word was: " + secretWord;
                return;
            }

            DisplayGuess(guess);
            GuessEntry.Text = string.Empty;
            currentAttempt++;

            if (guess == secretWord)
            {
                EndGame(secretWord, currentAttempt);
            }
            else if (currentAttempt >= attempts)
            {
                EndGame(secretWord, currentAttempt);
            }
        }

        private void DisplayGuess(string guess)
        {
            Console.WriteLine($"Current Attempt: {currentAttempt}, Total Children: {GuessGrid.Children.Count}");

            if (currentAttempt < attempts)
            {
                for (int i = 0; i < 5; i++)
                {
                    int index = currentAttempt * 5 + i;

                    if (index < GuessGrid.Children.Count)
                    {
                        var label = (Label)GuessGrid.Children[index];
                        label.Text = guess[i].ToString();

                        if (guess[i] == secretWord[i])
                        {
                            label.TextColor = Colors.Green;
                        }
                        else if (secretWord.Contains(guess[i]))
                        {
                            label.TextColor = Colors.Orange;
                        }
                        else
                        {
                            label.TextColor = Colors.Red;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Index out of bounds: {index}");
                    }
                }
            }
        }

        private void EndGame(string correctWord, int guesses)
        {
            SaveGameAttempt(correctWord, guesses);

            if (guesses <= attempts)
            {
                DisplayAlert("Congratulations!", $"You've guessed the word '{correctWord}' in {guesses} attempts!", "OK");
            }
            else
            {
                DisplayAlert("Game Over", $"The correct word was: {correctWord}. You took {guesses} attempts.", "OK");
            }

            StartNewGame();
        }

        private void SaveGameAttempt(string correctWord, int guesses)
        {
            var gameAttempt = new GameAttempt(DateTime.Now, correctWord, guesses);
            var gameAttempts = LoadGameAttempts();
            gameAttempts.Add(gameAttempt);

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameHistory.json");
            string json = JsonSerializer.Serialize(gameAttempts);
            File.WriteAllText(filePath, json);
        }

        private List<GameAttempt> LoadGameAttempts()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameHistory.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<GameAttempt>>(json) ?? new List<GameAttempt>();
            }
            return new List<GameAttempt>();
        }

        private async void OnViewHistoryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
        }

        private void OnStartNewGameClicked(object sender, EventArgs e)
        {
            try
            {
                StartNewGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting new game: {ex.Message}");
                DisplayAlert("Error", "An error occurred while starting a new game. Please try again.", "OK");
            }
        }
    }
}