using Hangman_Game.Models;
using System.Collections.Generic;

namespace Hangman_Game.Services;

public interface IDialogService
{
    bool ConfirmDelete(string username);
    SavedGameState SelectSavedGame(List<SavedGameState> saves);
    void ShowAbout();
    void ShowMessage(string title, string message);
    void ShowError(string title, string message);
}
