using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Hangman_Game.Models;

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
                        try
                        {
                            var json = File.ReadAllText(file);
                            var stats = JsonSerializer.Deserialize<UserStatistics>(json);
                            if (stats != null && !string.IsNullOrWhiteSpace(stats.Username))
                            {
                                stats.CategoriesStats ??= new Dictionary<string, CategoryStats>();
                                allStats.Add(stats);
                            }
                            else
                            {
                                File.Delete(file);
                            }
                        }
                        catch
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
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var stats = JsonSerializer.Deserialize<UserStatistics>(json);
                    if (stats != null && stats.Username == username)
                    {
                        stats.CategoriesStats ??= new Dictionary<string, CategoryStats>();
                        return stats;
                    }
                }
            }
            catch (Exception)
            {
                try { if (File.Exists(filePath)) File.Delete(filePath); } catch { }
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

        try
        {
            string filePath = Path.Combine(_dataDirectory, $"{stats.Username}_userstats.json");
            var json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception)
        {
            // Fail gracefully on save
        }
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
