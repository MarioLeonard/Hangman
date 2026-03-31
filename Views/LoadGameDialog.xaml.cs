using System.Windows;
using Hangman_Game.Models;
using System.Collections.Generic;

namespace Hangman_Game.Views;

public partial class LoadGameDialog : Window
{
    public SavedGameState? SelectedSave { get; private set; }

    public LoadGameDialog(List<SavedGameState> saves)
    {
        InitializeComponent();
        SavesList.ItemsSource = saves;
    }

    private void SavesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        LoadButton.IsEnabled = SavesList.SelectedItem != null;
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        if (SavesList.SelectedItem is SavedGameState save)
        {
            SelectedSave = save;
            DialogResult = true;
            Close();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}