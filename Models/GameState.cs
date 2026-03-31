using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hangman_Game.Models;

public partial class GameState : ObservableObject
{
    [ObservableProperty]
    private string _currentWord = string.Empty;

    [ObservableProperty]
    private string _displayWord = string.Empty;

    [ObservableProperty]
    private ObservableCollection<char> _guessedLetters = new();

    [ObservableProperty]
    private int _attemptsLeft = 6;

    [ObservableProperty]
    private int _maxAttempts = 6;

    [ObservableProperty]
    private int _currentLevel = 1;

    [ObservableProperty]
    private int _consecutiveWins = 0;

    [ObservableProperty]
    private int _timeRemaining = 60;

    [ObservableProperty]
    private string _selectedCategory = "All Categories";

    [ObservableProperty]
    private string _status = "Playing";
}
