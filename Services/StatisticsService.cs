using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangman_Game.Models;
using Hangman_Game.Helpers;

namespace Hangman_Game.Services;

public class StatisticsService : IStatisticsService
{
    private readonly string _dataDirectory;
    private static readonly object _fileLock = new object();

    public StatisticsService()
    {
        _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Statistics");
        lock (_fileLock)
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }
    }

    public List<UserStatistics> GetAllStatistics()
    {
        var allStats = new List<UserStatistics>();
        lock (_fileLock)
        {
            try
            {
                if (Directory.Exists(_dataDirectory))
                {
                    var files = Directory.GetFiles(_dataDirectory, "*_userstats.json");
                    foreach (var file in files)
                    {
                        var stats = JsonFileHelper.Load<UserStatistics>(file);
                        if (stats != null && !string.IsNullOrWhiteSpace(stats.Username))
                        {
                            stats.CategoriesStats ??= new Dictionary<string, CategoryStats>();
                            allStats.Add(stats);
                        }
                        else
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                }
            }
            catch
            {
                // Ignore directory access errors
            }
        }
        return allStats;
    }

    public UserStatistics GetUserStatistics(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return new UserStatistics { Username = "Unknown", CategoriesStats = new Dictionary<string, CategoryStats>() };
        }

        lock (_fileLock)
        {
            string filePath = Path.Combine(_dataDirectory, $"{username}_userstats.json");
            if (File.Exists(filePath))
            {
                var stats = JsonFileHelper.Load<UserStatistics>(filePath);
                if (stats != null && stats.Username == username)
                {
                    stats.CategoriesStats ??= new Dictionary<string, CategoryStats>();
                    return stats;
                }
            }
        }
        
        return new UserStatistics { Username = username, CategoriesStats = new Dictionary<string, CategoryStats>() };
    }

    public void UpdateAfterGame(string username, string category, bool isWin)
    {
        if (string.IsNullOrWhiteSpace(username)) return;

        lock (_fileLock)
        {
            var stats = GetUserStatistics(username);

            stats.TotalGamesPlayed++;

            string safeCategory = string.IsNullOrWhiteSpace(category) ? "Uncategorized" : category;

            if (!stats.CategoriesStats.ContainsKey(safeCategory))
            {
                stats.CategoriesStats[safeCategory] = new CategoryStats { CategoryName = safeCategory };
            }

            stats.CategoriesStats[safeCategory].GamesPlayed++;
            if (isWin)
            {
                stats.CategoriesStats[safeCategory].GamesWon++;
            }

            SaveUserStatistics(stats);
        }
    }

    private void SaveUserStatistics(UserStatistics stats)
    {
        if (stats == null || string.IsNullOrWhiteSpace(stats.Username)) return;

        string filePath = Path.Combine(_dataDirectory, $"{stats.Username}_userstats.json");
        JsonFileHelper.Save(filePath, stats);
    }

    public void DeleteUserStatistics(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return;

        lock (_fileLock)
        {
            string filePath = Path.Combine(_dataDirectory, $"{username}_userstats.json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception)
            {
                // Fail gracefully
            }
        }
    }
}
