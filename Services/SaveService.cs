using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Hangman_Game.Models;

namespace Hangman_Game.Services;

public class SaveService : ISaveService
{
    private readonly string _saveDirectory;

    public SaveService()
    {
        _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Saves");
        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
        }
    }

    private string GetUserDirectory(string username)
    {
        var dir = Path.Combine(_saveDirectory, username);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    public async Task SaveGameAsync(SavedGameState gameState)
    {
        try
        {
            if (gameState.Id == Guid.Empty)
            {
                gameState.Id = Guid.NewGuid();
            }
            if (gameState.SavedAt == default)
            {
                gameState.SavedAt = DateTime.Now;
            }

            var userDir = GetUserDirectory(gameState.Username);
            string filePath = Path.Combine(userDir, $"{gameState.Id}.json");
            var json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
            
            using var writer = new StreamWriter(filePath);
            await writer.WriteAsync(json);
        }
        catch (Exception)
        {
            throw new Exception("Failed to save the game.");
        }
    }

    public void SaveGame(SavedGameState gameState)
    {
        try
        {
            if (gameState.Id == Guid.Empty)
            {
                gameState.Id = Guid.NewGuid();
            }
            if (gameState.SavedAt == default)
            {
                gameState.SavedAt = DateTime.Now;
            }

            var userDir = GetUserDirectory(gameState.Username);
            string filePath = Path.Combine(userDir, $"{gameState.Id}.json");
            var json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception)
        {
            // Fail gracefully on write error
        }
    }

    public SavedGameState? LoadGame(string username, Guid saveId)
    {
        try
        {
            var userDir = GetUserDirectory(username);
            string filePath = Path.Combine(userDir, $"{saveId}.json");
            
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SavedGameState>(json);
        }
        catch (Exception)
        {
            // Return null to signify no successful restore if corrupted
            return null;
        }
    }

    public List<SavedGameState> GetAllSaves(string username)
    {
        var saves = new List<SavedGameState>();
        try
        {
            var userDir = GetUserDirectory(username);
            var files = Directory.GetFiles(userDir, "*.json");
            
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var state = JsonSerializer.Deserialize<SavedGameState>(json);
                    if (state != null)
                    {
                        saves.Add(state);
                    }
                }
                catch
                {
                    // Skip unreadable files
                }
            }
        }
        catch (Exception)
        {
            // Return empty list on failure
        }
        
        return saves.OrderByDescending(s => s.SavedAt).ToList();
    }

    public void DeleteSave(string username, Guid saveId)
    {
        try
        {
            var userDir = GetUserDirectory(username);
            string filePath = Path.Combine(userDir, $"{saveId}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception)
        {
            // Ignore failure
        }
    }

    public void DeleteAllSaves(string username)
    {
        try
        {
            var userDir = Path.Combine(_saveDirectory, username);
            if (Directory.Exists(userDir))
            {
                Directory.Delete(userDir, true);
            }
        }
        catch (Exception)
        {
            // Ignore failure
        }
    }
}
