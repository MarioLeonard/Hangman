using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hangman_Game.Models;
using Hangman_Game.Services;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;

namespace Hangman_Game.ViewModels;

public partial class GameViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IWordRepository _wordRepository;
    private readonly ISaveService _saveService;
    private readonly IStatisticsService _statisticsService;
    private readonly IDialogService _dialogService;
    private readonly Random _random = new();
    private DispatcherTimer? _gameTimer;

    private Dictionary<string, List<string>> _wordPools = new();
    private Dictionary<string, List<string>> _availableWords = new();

    private UserStatistics? _currentUserStats;

    [ObservableProperty]
    private GameState _gameState = new();

    [ObservableProperty]
    private BitmapImage? _currentHangmanImage;

    private readonly List<BitmapImage?> _hangmanImages = new();

    public User CurrentUser { get; }

    [ObservableProperty]
    private ObservableCollection<LetterItem> _alphabet = new();

    public GameViewModel(User user, INavigationService navigationService, IWordRepository wordRepository, ISaveService saveService, IStatisticsService statisticsService, IDialogService dialogService)
    {
        CurrentUser = user;
        _navigationService = navigationService;
        _wordRepository = wordRepository;
        _saveService = saveService;
        _statisticsService = statisticsService;
        _dialogService = dialogService;
        
        _currentUserStats = _statisticsService.GetUserStatistics(user.Username);
        
        LoadWordPools();
        InitializeTimer();
        InitializeHangmanImages();
        NewGame();
    }

    private void LoadWordPools()
    {
        _wordPools = _wordRepository.LoadCategories();
        ResetAvailableWords();
    }
    
    private void ResetAvailableWords()
    {
        _availableWords = _wordPools.ToDictionary(k => k.Key, v => new List<string>(v.Value));
    }

    private void InitializeTimer()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _gameTimer.Tick += GameTimer_Tick;
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        if (GameState.Status != "Playing") return;

        GameState.TimeRemaining--;

        if (GameState.TimeRemaining <= 0)
        {
            HandleLevelLoss();
        }
    }

    private void InitializeAlphabet()
    {
        Alphabet.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            Alphabet.Add(new LetterItem { Character = c, IsGuessed = false, IsCorrect = null });
        }
    }

    private void InitializeHangmanImages()
    {
        _hangmanImages.Clear();
        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Hangman");
        
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        // Preload 0 to 6 images to avoid IO bounds during active gameplay tracking attempts left.
        for (int i = 0; i <= 6; i++)
        {
            string filePath = Path.Combine(basePath, $"{i}.png");
            if (File.Exists(filePath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Preload entirely to memory
                bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze();
                _hangmanImages.Add(bitmap);
            }
            else
            {
                _hangmanImages.Add(null);
            }
        }
    }

    [RelayCommand]
    private void GuessLetter(object parameter)
    {
        if (parameter == null) return;
        
        string paramStr = parameter.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(paramStr)) return;
        
        char letter = paramStr.ToUpper()[0];

        if (GameState.Status != "Playing") return;

        var letterItem = Alphabet.FirstOrDefault(l => l.Character == letter);
        if (letterItem == null || letterItem.IsGuessed) return;

        letterItem.IsGuessed = true;
        GameState.GuessedLetters.Add(letter);

        if (GameState.CurrentWord.Contains(letter))
        {
            letterItem.IsCorrect = true;
            UpdateDisplayedWord();
            if (!GameState.DisplayWord.Contains('_'))
            {
                HandleLevelWin();
            }
        }
        else
        {
            letterItem.IsCorrect = false;
            GameState.AttemptsLeft--;
            UpdateHangmanImage();
            if (GameState.AttemptsLeft <= 0)
            {
                HandleLevelLoss();
            }
        }
    }

    private void UpdateHangmanImage()
    {
        int index = GameState.MaxAttempts - GameState.AttemptsLeft;
        if (index >= 0 && index < _hangmanImages.Count)
        {
            CurrentHangmanImage = _hangmanImages[index];
        }
    }

    private void UpdateDisplayedWord()
    {
        GameState.DisplayWord = string.Join(" ", GameState.CurrentWord.Select(c => GameState.GuessedLetters.Contains(c) ? c : '_'));
    }

    private void HandleLevelWin()
    {
        _gameTimer?.Stop();
        GameState.ConsecutiveWins++;

        if (GameState.ConsecutiveWins >= 3)
        {
            GameState.Status = "Game Won";
            UpdateUserStatistics(true);
        }
        else
        {
            StartLevel(GameState.SelectedCategory ?? "All Categories", GameState.CurrentLevel + 1, GameState.ConsecutiveWins);
        }
    }

    private void HandleLevelLoss()
    {
        _gameTimer?.Stop();
        
        if (GameState.ConsecutiveWins < 3) 
        {
             UpdateUserStatistics(false);
        }

        GameState.ConsecutiveWins = 0;
        GameState.Status = "Lost";
        GameState.DisplayWord = string.Join(" ", GameState.CurrentWord.ToCharArray()); // Reveal word
    }

    private void UpdateUserStatistics(bool isWon)
    {
        string category = GameState.SelectedCategory ?? "All Categories";
        _statisticsService.UpdateAfterGame(CurrentUser.Username, category, isWon);
        _currentUserStats = _statisticsService.GetUserStatistics(CurrentUser.Username);
    }

    [RelayCommand]
    private void NextLevel()
    {
        if (GameState.Status != "Level Won") return;

        var prevLevel = GameState.CurrentLevel + 1;
        var prevWins = GameState.ConsecutiveWins;
        var currentCategory = GameState.SelectedCategory;
        
        StartLevel(currentCategory, prevLevel, prevWins);
    }

    [RelayCommand]
    private void NewGame()
    {
        var category = GameState.SelectedCategory ?? "All Categories";
        StartLevel(category, 1, 0);
    }

    private void StartLevel(string category, int level, int consecutiveWins)
    {
        string newWord = GetRandomWord(category);
        
        GameState = new GameState
        {
            CurrentWord = newWord,
            MaxAttempts = 6,
            AttemptsLeft = 6,
            CurrentLevel = level,
            ConsecutiveWins = consecutiveWins,
            SelectedCategory = category,
            TimeRemaining = 30, // Updated logic required starting at 30
            Status = "Playing"
        };
        
        // Reset GuessedLetters here so the display word correctly recalculates
        GameState.GuessedLetters.Clear();

        InitializeAlphabet();
        UpdateHangmanImage();
        UpdateDisplayedWord();
        _gameTimer?.Start();
    }

    private string GetRandomWord(string category)
    {
        List<string> pool;
        List<string> backupPool;
        
        if (category == "All Categories")
        {
            pool = _availableWords.Values.SelectMany(x => x).ToList();
            backupPool = _wordPools.Values.SelectMany(x => x).ToList();
        }
        else if (_availableWords.ContainsKey(category))
        {
            pool = _availableWords[category];
            backupPool = _wordPools[category];
        }
        else
        {
            return "DEFAULT"; // Edge-case
        }

        // If pool is empty (no repetition enhancement triggered), reset the specific active tracker pool so we don't softly crash.
        if (pool.Count == 0)
        {
            if (category == "All Categories")
            {
               ResetAvailableWords();
               pool = _availableWords.Values.SelectMany(x => x).ToList();
            }
            else
            {
                _availableWords[category] = new List<string>(_wordPools[category]);
                pool = _availableWords[category];
            }
            
            // Re-check just in case the original data file pool was literally empty
            if (pool.Count == 0) return "DEFAULT"; 
        }

        var pickedWord = pool[_random.Next(pool.Count)];
        
        // Anti-repetition logic: remove the picked word from all tracking pools it exists in
        foreach(var availableList in _availableWords.Values)
        {
            availableList.Remove(pickedWord);
        }

        return pickedWord;
    }

    [RelayCommand]
    private void OpenGame()
    {
        _gameTimer?.Stop();
        
        var saves = _saveService.GetAllSaves(CurrentUser.Username);
        
        if (saves.Count == 0)
        {
            _dialogService.ShowMessage("No Saves", "You don't have any saved games.");
            if (GameState.Status == "Playing")
            {
                _gameTimer?.Start();
            }
            return;
        }

        var loadedData = _dialogService.SelectSavedGame(saves);
        
        if (loadedData != null)
        {
            GameState = new GameState
            {
                CurrentWord = loadedData.CurrentWord,
                DisplayWord = loadedData.DisplayWord,
                GuessedLetters = new ObservableCollection<char>(loadedData.GuessedLetters ?? new List<char>()),
                AttemptsLeft = loadedData.AttemptsLeft,
                MaxAttempts = 6, // Hardcoded requirement for mapping base constraints seamlessly
                CurrentLevel = loadedData.CurrentLevel,
                ConsecutiveWins = loadedData.ConsecutiveWins,
                SelectedCategory = loadedData.SelectedCategory,
                TimeRemaining = loadedData.TimeRemaining,
                Status = loadedData.Status
            };

            // Restore letter alphabet state mapping correct / incorrect UI representations
            InitializeAlphabet();
            foreach (var letter in Alphabet)
            {
                if (GameState.GuessedLetters.Contains(letter.Character))
                {
                    letter.IsGuessed = true;
                    // Handle edge cases where current word might be null/corrupted
                    letter.IsCorrect = GameState.CurrentWord != null && GameState.CurrentWord.Contains(letter.Character);
                }
            }

            UpdateHangmanImage();
            
            if (GameState.Status == "Playing")
            {
                _gameTimer?.Start();
            }
        }
        else
        {
            if (GameState.Status == "Playing")
            {
                _gameTimer?.Start(); // Resume seamlessly if loading was cancelled
            }
        }
    }

    [RelayCommand]
    private async Task SaveGame()
    {
        if (GameState.Status != "Playing")
        {
            _dialogService.ShowError("Save Failed", "You can only save an active game.");
            return;
        }

        var activeSaveData = new SavedGameState
        {
            Username = CurrentUser.Username,
            CurrentWord = GameState.CurrentWord,
            DisplayWord = GameState.DisplayWord,
            GuessedLetters = GameState.GuessedLetters.ToList(),
            AttemptsLeft = GameState.AttemptsLeft,
            CurrentLevel = GameState.CurrentLevel,
            ConsecutiveWins = GameState.ConsecutiveWins,
            SelectedCategory = GameState.SelectedCategory,
            TimeRemaining = GameState.TimeRemaining,
            Status = GameState.Status
        };
        
        try
        {
            await _saveService.SaveGameAsync(activeSaveData);
            _dialogService.ShowMessage("Game Saved", "Your game has been saved successfully.");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError("Save Error", $"An error occurred while saving the game:\n{ex.Message}");
        }
    }

    [RelayCommand]
    private void ShowStatistics()
    {
        _gameTimer?.Stop(); // Automatically pause the game while statistics are being viewed to avoid background timer tracking losses
        
        _navigationService.NavigateToStatistics(CurrentUser);
        
        if (GameState.Status == "Playing")
        {
            _gameTimer?.Start(); 
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _gameTimer?.Stop();
        _navigationService.NavigateToStart();
    }

    [RelayCommand]
    private void ChangeCategory(string category)
    {
        if (category != GameState.SelectedCategory)
        {
            GameState.SelectedCategory = category;
            ResetAvailableWords(); // Optional specific enhancement: Resets repetition tracking on category swap naturally.
            NewGame(); // Changing category resets game naturally handling streak wiping per requirements.
        }
    }

    [RelayCommand]
    private void ShowAbout()
    {
        _gameTimer?.Stop(); // Automatically pause the game
        
        _dialogService.ShowAbout();
        
        if (GameState.Status == "Playing")
        {
            _gameTimer?.Start(); 
        }
    }
}
