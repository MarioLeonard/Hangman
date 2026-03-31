namespace Hangman_Game.Models;

public class SavedGameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string CurrentWord { get; set; } = string.Empty;
    public string DisplayWord { get; set; } = string.Empty;
    public List<char> GuessedLetters { get; set; } = new();
    public int AttemptsLeft { get; set; }
    public int CurrentLevel { get; set; }
    public int ConsecutiveWins { get; set; }
    public string SelectedCategory { get; set; } = string.Empty;
    public int TimeRemaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; } = DateTime.Now;
}
