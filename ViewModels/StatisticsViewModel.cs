using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hangman_Game.Services;
using Hangman_Game.Models;
using System.Linq;
using System.Windows.Input;

namespace Hangman_Game.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly IStatisticsService _statisticsService;
    private readonly INavigationService _navigationService;
    private readonly User _currentUser;

    [ObservableProperty]
    private ObservableCollection<UserCategoryStatViewModel> _usersStatistics = new();

    public StatisticsViewModel(IStatisticsService statisticsService, INavigationService navigationService, User currentUser = null)
    {
        _statisticsService = statisticsService;
        _navigationService = navigationService;
        _currentUser = currentUser;
        LoadStatistics();
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadStatistics();
    }

    [RelayCommand]
    private void Close()
    {
        if (_currentUser != null)
        {
            _navigationService.NavigateToGame(_currentUser);
        }
        else
        {
            _navigationService.NavigateToStart();
        }
    }

    private void LoadStatistics()
    {
        UsersStatistics.Clear();
        var rawStats = _statisticsService.GetAllStatistics();
        
        var flatStats = rawStats
            .SelectMany(userStat => userStat.CategoriesStats.Values.Select(categoryStat => new UserCategoryStatViewModel
            {
                Username = userStat.Username,
                Category = categoryStat.CategoryName,
                GamesPlayed = categoryStat.GamesPlayed,
                GamesWon = categoryStat.GamesWon
            }))
            .OrderBy(s => s.Username)
            .ThenBy(s => s.Category);

        foreach (var stat in flatStats)
        {
            UsersStatistics.Add(stat);
        }
    }
}

public class UserCategoryStatViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
}