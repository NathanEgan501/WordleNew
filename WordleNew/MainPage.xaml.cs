// g00435730 Nathan Egan
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;

namespace WordleNew
{
    public partial class MainPage : ContentPage
    {
        private string secretWord;
        private int attempts;
        private List<string> wordList = new List<string>();
        private int currentAttempt;

        public MainPage()
        {
            InitializeComponent();
            LoadWordList();
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
                    var label = (Label)GuessGrid.Children[i * 5 + j];
                    label.Text = string.Empty;
                    label.TextColor = Colors.Black;
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
                MessageLabel.Text = "Congratulations! You've guessed the word!";
            }
            else if (currentAttempt >= attempts)
            {
                MessageLabel.Text = "Game Over! The word was: " + secretWord;
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

    }

}