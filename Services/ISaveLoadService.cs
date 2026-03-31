using Hangman_Game.Models;

namespace Hangman_Game.Services;

public interface ISaveLoadService
{
    void SaveGame(SavedGameState game);
    List<SavedGameState> LoadGames(string username);
    SavedGameState? LoadGame(string saveId, string username);
    void DeleteSave(string saveId, string username);
    void DeleteAllSaves(string username);
}
