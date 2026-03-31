using System.Windows;
using Hangman_Game.Models;
using System.Collections.Generic;
using Hangman_Game.Views;

namespace Hangman_Game.Services;

public class DialogService : IDialogService
{
    public bool ConfirmDelete(string username)
    {
        var result = MessageBox.Show(
            $"Are you sure you want to delete the user '{username}' and ALL associated data?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        return result == MessageBoxResult.Yes;
    }

    public void ShowStatistics(PlayerStatistics stats)
    {
        var message = $"Username: {stats.Username}\n" +
                      $"Total Games Played: {stats.TotalGamesPlayed}\n" +
                      $"Total Games Won: {stats.TotalGamesWon}\n" +
                      $"Total Levels Completed: {stats.TotalLevelsCompleted}\n" +
                      $"Win Rate: {stats.WinRate:F2}%";

        MessageBox.Show(message, "Player Statistics", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowAbout()
    {
        var message = "Hangman Game\n\n" +
                      "Student Name - Popa Mario Leonard\n" +
                      "Group Number - 10LF343\n" +
                      "Specialization - Applied Computer Science";

        MessageBox.Show(message, "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    public void ShowError(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public SavedGameState? SelectSavedGame(List<SavedGameState> saves)
    {
        var dialog = new LoadGameDialog(saves);
        if (Application.Current.MainWindow != null)
        {
            dialog.Owner = Application.Current.MainWindow;
        }
        
        if (dialog.ShowDialog() == true)
        {
            return dialog.SelectedSave;
        }
        
        return null;
    }
}
