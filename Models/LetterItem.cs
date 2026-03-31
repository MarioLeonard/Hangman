using CommunityToolkit.Mvvm.ComponentModel;

namespace Hangman_Game.Models;

public partial class LetterItem : ObservableObject
{
    [ObservableProperty]
    private char _character;

    [ObservableProperty]
    private bool _isGuessed;

    [ObservableProperty]
    private bool? _isCorrect;
}
