using System.IO;
using System.Text.Json;

namespace Hangman_Game.Services;

public class WordRepository : IWordRepository
{
    private readonly string _dataDirectory;
    private readonly string _filePath;
    
    // Fallback defaults if file doesn't exist yet
    private readonly Dictionary<string, List<string>> _defaultPools = new()
    {
        { "Cars", new List<string> { "TOYOTA", "FERRARI", "PORSCHE", "FORD", "HONDA", "CHEVROLET", "TESLA", "BMW", "MERCEDES", "AUDI" } },
        { "Movies", new List<string> { "INCEPTION", "AVATAR", "GLADIATOR", "ALIEN", "TITANIC", "MATRIX", "BATMAN", "INTERSTELLAR", "JAWS", "ROCKY" } },
        { "Rivers", new List<string> { "AMAZON", "NILE", "MISSISSIPPI", "THAMES", "DANUBE", "YANGTZE", "VOLGA", "GANGES", "COLORADO", "SEINE" } }
    };

    public WordRepository()
    {
        _dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        _filePath = Path.Combine(_dataDirectory, "words.json");
    }

    public Dictionary<string, List<string>> LoadCategories()
    {
        try
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            if (!File.Exists(_filePath))
            {
                SaveDefaults();
                return _defaultPools;
            }

            var json = File.ReadAllText(_filePath);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            
            return parsed != null && parsed.Count > 0 ? parsed : _defaultPools;
        }
        catch (Exception)
        {
            return _defaultPools;
        }
    }

    private void SaveDefaults()
    {
        try
        {
            var json = JsonSerializer.Serialize(_defaultPools, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception)
        {
            // Ignore write fail on defaults if permissions block
        }
    }
}
