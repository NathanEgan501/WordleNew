using System;

namespace WordleNew
{
    public class GameAttempt
    {
        public DateTime Timestamp { get; set; }
        public string CorrectWord { get; set; } 
        public int Guesses { get; set; }

        public GameAttempt(DateTime timestamp, string correctWord, int guesses)
        {
            Timestamp = timestamp;
            CorrectWord = correctWord;
            Guesses = guesses;
        }
    }
}