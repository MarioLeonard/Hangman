using Hangman_Game.Models;
using System.Collections.Generic;

namespace Hangman_Game.Services;

public interface IDialogService
{
    bool ConfirmDelete(string username);
    void ShowStatistics(PlayerStatistics stats);
    void ShowAbout();
    void ShowMessage(string title, string message);
    void ShowError(string title, string message);
    SavedGameState? SelectSavedGame(List<SavedGameState> saves);
}
