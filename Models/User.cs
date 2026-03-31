using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Hangman_Game.Models;

public partial class User : ObservableObject
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _imagePath = string.Empty;
}
