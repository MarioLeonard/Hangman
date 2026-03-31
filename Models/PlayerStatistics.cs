using System.Collections.Generic;

namespace Hangman_Game.Models;

public class CategoryStats
{
    public string CategoryName { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
}

public class UserStatistics
{
    public string Username { get; set; } = string.Empty;
    public int TotalGamesPlayed { get; set; }
    public Dictionary<string, CategoryStats> CategoriesStats { get; set; } = new();
}
