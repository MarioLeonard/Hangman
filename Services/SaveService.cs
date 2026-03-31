using System.IO;
using System.Threading.Tasks;
using Hangman_Game.Helpers;
using Hangman_Game.Models;

namespace Hangman_Game.Services;

public class SaveService : ISaveService
{
    private readonly string _saveDirectory;

    public SaveService()
    {
        _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Saves");
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
        
        // Simulating async by wrapping the sync helper call to maintain interface signature
        await Task.Run(() => JsonFileHelper.Save(filePath, gameState));
    }

    public void SaveGame(SavedGameState gameState)
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
        JsonFileHelper.Save(filePath, gameState);
    }

    public SavedGameState? LoadGame(string username, Guid saveId)
    {
        var userDir = GetUserDirectory(username);
        string filePath = Path.Combine(userDir, $"{saveId}.json");
        
        return JsonFileHelper.Load<SavedGameState>(filePath);
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
                var state = JsonFileHelper.Load<SavedGameState>(file);
                if (state != null)
                {
                    saves.Add(state);
                }
            }
        }
        catch (Exception)
        {
            // Return empty list on directory read failure
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
