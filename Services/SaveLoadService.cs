using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Hangman_Game.Models;

namespace Hangman_Game.Services;

public class SaveLoadService : ISaveLoadService
{
    private readonly string _saveDirectory;

    public SaveLoadService()
    {
        _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Saves");
    }

    private string GetUserDirectory(string username)
    {
        return Path.Combine(_saveDirectory, username);
    }

    public void SaveGame(SavedGameState game)
    {
        var userDir = GetUserDirectory(game.Username);
        if (!Directory.Exists(userDir))
        {
            Directory.CreateDirectory(userDir);
        }

        string filePath = Path.Combine(userDir, $"{game.Id}.json");
        var json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public List<SavedGameState> LoadGames(string username)
    {
        var saves = new List<SavedGameState>();
        var userDir = GetUserDirectory(username);
        
        if (!Directory.Exists(userDir))
        {
            return saves;
        }

        var files = Directory.GetFiles(userDir, "*.json");
        foreach (var file in files)
        {
            try
            {
                var json = File.ReadAllText(file);
                var state = JsonSerializer.Deserialize<SavedGameState>(json);
                if (state != null && ValidateGameState(state))
                {
                    saves.Add(state);
                }
                else
                {
                    // Auto-clean corrupted/invalid saves
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                // Auto-clean corrupted saves (JSON parse error, etc.)
                try { File.Delete(file); } catch { }
            }
        }
        
        return saves;
    }

    public SavedGameState? LoadGame(string saveId, string username)
    {
        var userDir = GetUserDirectory(username);
        string filePath = Path.Combine(userDir, $"{saveId}.json");
        
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var state = JsonSerializer.Deserialize<SavedGameState>(json);
            if (state != null && ValidateGameState(state))
            {
                return state;
            }
            else
            {
                // Auto-clean invalid
                File.Delete(filePath);
                return null;
            }
        }
        catch (Exception)
        {
            // Auto-clean on error
            try { File.Delete(filePath); } catch { }
            return null;
        }
    }

    private bool ValidateGameState(SavedGameState state)
    {
        // Add robust validation logic to verify the game state is valid and complete
        if (string.IsNullOrEmpty(state.CurrentWord) || 
            string.IsNullOrEmpty(state.SelectedCategory) ||
            state.Id == Guid.Empty)
        {
            return false;
        }

        // Additional checks like verifying if category exists could be added here
        // if this service had access to IWordRepository or similar.

        return true;
    }

    public void DeleteSave(string saveId, string username)
    {
        var userDir = GetUserDirectory(username);
        string filePath = Path.Combine(userDir, $"{saveId}.json");
        
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // Ignore failure
            }
        }
    }

    public void DeleteAllSaves(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return;

        var userDir = GetUserDirectory(username);
        if (Directory.Exists(userDir))
        {
            try
            {
                Directory.Delete(userDir, true);
            }
            catch
            {
                // Ignore failure gracefully
            }
        }
    }
}
