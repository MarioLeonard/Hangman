using Hangman_Game.Models;
using System.Threading.Tasks;

namespace Hangman_Game.Services;

public interface ISaveService
{
    Task SaveGameAsync(SavedGameState gameState);
    void SaveGame(SavedGameState gameState);
    SavedGameState? LoadGame(string username, Guid saveId);
    List<SavedGameState> GetAllSaves(string username);
    void DeleteSave(string username, Guid saveId);
    void DeleteAllSaves(string username);
}
